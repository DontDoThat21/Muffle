using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing friend requests and friendships
    /// </summary>
    public static class FriendRequestService
    {
        /// <summary>
        /// Search for users by email or username
        /// </summary>
        /// <param name="searchTerm">Email or username to search for</param>
        /// <param name="currentUserId">ID of the current user (to exclude from results)</param>
        /// <returns>List of matching users</returns>
        public static List<User> SearchUsers(string searchTerm, int currentUserId)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<User>();
            }

            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var searchQuery = @"
                    SELECT * FROM Users 
                    WHERE (Email LIKE @SearchTerm OR Name LIKE @SearchTerm) 
                    AND UserId != @CurrentUserId
                    LIMIT 20;";

                var users = connection.Query<User>(searchQuery, new
                {
                    SearchTerm = $"%{searchTerm}%",
                    CurrentUserId = currentUserId
                }).ToList();

                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching users: {ex.Message}");
                return new List<User>();
            }
        }

        /// <summary>
        /// Send a friend request to a user
        /// </summary>
        /// <param name="senderId">ID of the user sending the request</param>
        /// <param name="receiverId">ID of the user receiving the request</param>
        /// <returns>The created FriendRequest, or null if failed</returns>
        public static FriendRequest? SendFriendRequest(int senderId, int receiverId)
        {
            if (senderId == receiverId)
            {
                Console.WriteLine("Cannot send friend request to yourself");
                return null;
            }

            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                // Check if a request already exists between these users
                var checkExistingQuery = @"
                    SELECT COUNT(*) FROM FriendRequests 
                    WHERE (SenderId = @SenderId AND ReceiverId = @ReceiverId)
                       OR (SenderId = @ReceiverId AND ReceiverId = @SenderId);";

                var existingCount = connection.ExecuteScalar<int>(checkExistingQuery, new
                {
                    SenderId = senderId,
                    ReceiverId = receiverId
                });

                if (existingCount > 0)
                {
                    Console.WriteLine("A friend request already exists between these users");
                    return null;
                }

                // Insert the friend request
                var insertQuery = @"
                    INSERT INTO FriendRequests (SenderId, ReceiverId, Status, CreatedAt)
                    VALUES (@SenderId, @ReceiverId, @Status, @CreatedAt);
                    SELECT last_insert_rowid();";

                var requestId = connection.QuerySingle<long>(insertQuery, new
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Status = (int)FriendRequestStatus.Pending,
                    CreatedAt = DateTime.Now
                });

                // Retrieve the created request with user details
                return GetFriendRequestById((int)requestId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending friend request: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get a friend request by its ID
        /// </summary>
        /// <param name="requestId">The request ID</param>
        /// <returns>The FriendRequest with user details, or null if not found</returns>
        public static FriendRequest? GetFriendRequestById(int requestId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var getRequestQuery = @"
                    SELECT 
                        fr.RequestId,
                        fr.SenderId,
                        fr.ReceiverId,
                        fr.Status,
                        fr.CreatedAt,
                        fr.RespondedAt,
                        sender.Name as SenderName,
                        sender.Email as SenderEmail,
                        receiver.Name as ReceiverName,
                        receiver.Email as ReceiverEmail
                    FROM FriendRequests fr
                    INNER JOIN Users sender ON fr.SenderId = sender.UserId
                    INNER JOIN Users receiver ON fr.ReceiverId = receiver.UserId
                    WHERE fr.RequestId = @RequestId;";

                var request = connection.QueryFirstOrDefault<FriendRequest>(getRequestQuery, new { RequestId = requestId });
                return request;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting friend request: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get all pending friend requests for a user (both sent and received)
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of pending friend requests</returns>
        public static List<FriendRequest> GetPendingFriendRequests(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var getRequestsQuery = @"
                    SELECT 
                        fr.RequestId,
                        fr.SenderId,
                        fr.ReceiverId,
                        fr.Status,
                        fr.CreatedAt,
                        fr.RespondedAt,
                        sender.Name as SenderName,
                        sender.Email as SenderEmail,
                        receiver.Name as ReceiverName,
                        receiver.Email as ReceiverEmail
                    FROM FriendRequests fr
                    INNER JOIN Users sender ON fr.SenderId = sender.UserId
                    INNER JOIN Users receiver ON fr.ReceiverId = receiver.UserId
                    WHERE (fr.SenderId = @UserId OR fr.ReceiverId = @UserId)
                    AND fr.Status = @Status
                    ORDER BY fr.CreatedAt DESC;";

                var requests = connection.Query<FriendRequest>(getRequestsQuery, new
                {
                    UserId = userId,
                    Status = (int)FriendRequestStatus.Pending
                }).ToList();

                return requests;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting pending friend requests: {ex.Message}");
                return new List<FriendRequest>();
            }
        }

        /// <summary>
        /// Accept a friend request
        /// </summary>
        /// <param name="requestId">The request ID to accept</param>
        /// <param name="userId">The user ID accepting the request (must be the receiver)</param>
        /// <returns>True if accepted successfully, false otherwise</returns>
        public static bool AcceptFriendRequest(int requestId, int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                // Verify the user is the receiver of this request
                var verifyQuery = @"
                    SELECT COUNT(*) FROM FriendRequests 
                    WHERE RequestId = @RequestId 
                    AND ReceiverId = @UserId 
                    AND Status = @PendingStatus;";

                var isValid = connection.ExecuteScalar<int>(verifyQuery, new
                {
                    RequestId = requestId,
                    UserId = userId,
                    PendingStatus = (int)FriendRequestStatus.Pending
                }) > 0;

                if (!isValid)
                {
                    Console.WriteLine("Invalid friend request or user not authorized");
                    return false;
                }

                // Update the request status
                var updateQuery = @"
                    UPDATE FriendRequests 
                    SET Status = @AcceptedStatus, RespondedAt = @RespondedAt
                    WHERE RequestId = @RequestId;";

                connection.Execute(updateQuery, new
                {
                    RequestId = requestId,
                    AcceptedStatus = (int)FriendRequestStatus.Accepted,
                    RespondedAt = DateTime.Now
                });

                // TODO: Create actual friendship record in Friends table (to be implemented in future feature)

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting friend request: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Decline a friend request
        /// </summary>
        /// <param name="requestId">The request ID to decline</param>
        /// <param name="userId">The user ID declining the request (must be the receiver)</param>
        /// <returns>True if declined successfully, false otherwise</returns>
        public static bool DeclineFriendRequest(int requestId, int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                // Verify the user is the receiver of this request
                var verifyQuery = @"
                    SELECT COUNT(*) FROM FriendRequests 
                    WHERE RequestId = @RequestId 
                    AND ReceiverId = @UserId 
                    AND Status = @PendingStatus;";

                var isValid = connection.ExecuteScalar<int>(verifyQuery, new
                {
                    RequestId = requestId,
                    UserId = userId,
                    PendingStatus = (int)FriendRequestStatus.Pending
                }) > 0;

                if (!isValid)
                {
                    Console.WriteLine("Invalid friend request or user not authorized");
                    return false;
                }

                // Update the request status
                var updateQuery = @"
                    UPDATE FriendRequests 
                    SET Status = @DeclinedStatus, RespondedAt = @RespondedAt
                    WHERE RequestId = @RequestId;";

                connection.Execute(updateQuery, new
                {
                    RequestId = requestId,
                    DeclinedStatus = (int)FriendRequestStatus.Declined,
                    RespondedAt = DateTime.Now
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error declining friend request: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cancel a friend request (sender can cancel their own sent request)
        /// </summary>
        /// <param name="requestId">The request ID to cancel</param>
        /// <param name="userId">The user ID cancelling the request (must be the sender)</param>
        /// <returns>True if cancelled successfully, false otherwise</returns>
        public static bool CancelFriendRequest(int requestId, int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                // Verify the user is the sender of this request
                var verifyQuery = @"
                    SELECT COUNT(*) FROM FriendRequests 
                    WHERE RequestId = @RequestId 
                    AND SenderId = @UserId 
                    AND Status = @PendingStatus;";

                var isValid = connection.ExecuteScalar<int>(verifyQuery, new
                {
                    RequestId = requestId,
                    UserId = userId,
                    PendingStatus = (int)FriendRequestStatus.Pending
                }) > 0;

                if (!isValid)
                {
                    Console.WriteLine("Invalid friend request or user not authorized");
                    return false;
                }

                // Update the request status
                var updateQuery = @"
                    UPDATE FriendRequests 
                    SET Status = @CancelledStatus, RespondedAt = @RespondedAt
                    WHERE RequestId = @RequestId;";

                connection.Execute(updateQuery, new
                {
                    RequestId = requestId,
                    CancelledStatus = (int)FriendRequestStatus.Cancelled,
                    RespondedAt = DateTime.Now
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling friend request: {ex.Message}");
                return false;
            }
        }
    }
}

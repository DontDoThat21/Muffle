using System;
using System.Threading.Tasks;
using Muffle.Data.Models;
using Muffle.Data.Services;
using System.Text.Json;

namespace Muffle.Tests
{
    /// <summary>
    /// Simple demonstration of image message functionality
    /// </summary>
    public class ImageMessageDemo
    {
        public static async Task RunDemo()
        {
            Console.WriteLine("=== Muffle Image Message Demo ===");
            Console.WriteLine();

            // Create test data
            var user = new User { UserId = 1, Name = "Alice", Email = "alice@example.com", PasswordHash = "test" };
            var friend = new User { UserId = 2, Name = "Bob", Email = "bob@example.com", PasswordHash = "test" };

            // Create and send a text message
            Console.WriteLine("1. Creating text message...");
            var textMessage = new ChatMessage
            {
                Content = "Hello Bob! How are you?",
                Sender = user,
                Timestamp = DateTime.Now,
                Type = MessageType.Text
            };
            
            var textWrapper = new MessageWrapper
            {
                Type = MessageType.Text,
                Content = textMessage.Content,
                Timestamp = textMessage.Timestamp,
                SenderName = user.Name,
                SenderId = user.UserId
            };

            string textJson = JsonSerializer.Serialize(textWrapper);
            Console.WriteLine($"Text message JSON: {textJson}");
            Console.WriteLine();

            // Create and send an image message
            Console.WriteLine("2. Creating image message...");
            
            // Simulate image data
            string imagePath = "vacation_photo.jpg";
            string imageData = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"IMAGE_DATA_FOR_{imagePath}_{DateTime.Now:yyyyMMddHHmmss}"));
            
            var imageMessage = new ChatMessage
            {
                Content = $"📷 Image: {imagePath}",
                Sender = user,
                Timestamp = DateTime.Now,
                Type = MessageType.Image,
                ImageData = imageData,
                ImagePath = imagePath
            };
            
            var imageWrapper = new MessageWrapper
            {
                Type = MessageType.Image,
                Content = imageMessage.Content,
                ImageData = imageData,
                Timestamp = imageMessage.Timestamp,
                SenderName = user.Name,
                SenderId = user.UserId
            };

            string imageJson = JsonSerializer.Serialize(imageWrapper);
            Console.WriteLine($"Image message JSON: {imageJson}");
            Console.WriteLine();

            // Demonstrate message deserialization
            Console.WriteLine("3. Testing message deserialization...");
            
            var deserializedText = JsonSerializer.Deserialize<MessageWrapper>(textJson);
            var deserializedImage = JsonSerializer.Deserialize<MessageWrapper>(imageJson);
            
            Console.WriteLine($"Deserialized text: {deserializedText?.Content} (Type: {deserializedText?.Type})");
            Console.WriteLine($"Deserialized image: {deserializedImage?.Content} (Type: {deserializedImage?.Type})");
            Console.WriteLine($"Image data length: {deserializedImage?.ImageData?.Length} characters");
            Console.WriteLine();

            // Test image data round-trip
            Console.WriteLine("4. Testing image data round-trip...");
            if (!string.IsNullOrEmpty(deserializedImage?.ImageData))
            {
                try
                {
                    byte[] decodedBytes = Convert.FromBase64String(deserializedImage.ImageData);
                    string decodedContent = System.Text.Encoding.UTF8.GetString(decodedBytes);
                    Console.WriteLine($"Decoded image data: {decodedContent}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error decoding image data: {ex.Message}");
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("=== Demo Complete ===");
        }
    }
}
# Image Support in Muffle Chat

This document describes the new image support functionality added to Muffle's friend-to-friend chat system.

## Overview

The chat system now supports sending and receiving images alongside text messages. Images are transmitted as base64-encoded data through the WebSocket signaling server.

## Architecture

### Data Models

#### MessageType Enum
```csharp
public enum MessageType
{
    Text,
    Image
}
```

#### Enhanced ChatMessage
```csharp
public class ChatMessage
{
    public string Content { get; set; }
    public User Sender { get; set; }
    public DateTime Timestamp { get; set; }
    public MessageType Type { get; set; } = MessageType.Text;
    public string? ImagePath { get; set; }
    public string? ImageData { get; set; } // Base64 encoded image data
}
```

#### MessageWrapper for WebSocket Transmission
```csharp
public class MessageWrapper
{
    public MessageType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageData { get; set; }
    public DateTime Timestamp { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public int SenderId { get; set; }
}
```

### Services

#### ISignalingService Enhancements
- `SendMessageWrapperAsync(MessageWrapper messageWrapper)` - Send structured message
- `ReceiveMessageWrapperAsync()` - Receive structured message
- Maintains backward compatibility with existing `SendMessageAsync(string)` and `ReceiveMessageAsync()`

#### IImagePickerService
```csharp
public interface IImagePickerService
{
    Task<string?> PickImageAsync();
    Task<byte[]?> ConvertImageToByteArrayAsync(string imagePath);
    string ConvertByteArrayToBase64(byte[] imageBytes);
}
```

### UI Components

#### Updated Chat Interface
- Image button (📷) for sending images
- Custom DataTemplate for displaying different message types
- Value converters for proper image/text display

#### Value Converters
- `MessageTypeToTextVisibilityConverter` - Shows text for text messages
- `MessageTypeToImageVisibilityConverter` - Shows image display for image messages
- `Base64ToImageConverter` - Converts base64 data to ImageSource
- `StringNotEmptyConverter` - Determines when to show image elements

## Usage

### Sending Images

1. **UI Interaction**: User taps the 📷 button in the chat interface
2. **Image Selection**: `ImagePickerService.PickImageAsync()` is called (currently placeholder implementation)
3. **Image Processing**: Selected image is converted to byte array and then base64 string
4. **Message Creation**: `MessageWrapper` is created with `Type = MessageType.Image`
5. **Transmission**: Message is serialized to JSON and sent via WebSocket
6. **Local Display**: Chat message is added to the local chat list

### Receiving Images

1. **Message Reception**: JSON message received via WebSocket
2. **Deserialization**: JSON parsed into `MessageWrapper`
3. **Type Detection**: Message type determines display method
4. **UI Update**: Appropriate template shown based on message type
5. **Image Display**: Base64 data converted to ImageSource for display

### Code Example

```csharp
// Sending an image message
public async Task SendImageAsync()
{
    string? imagePath = await _imagePickerService.PickImageAsync();
    if (imagePath != null)
    {
        byte[]? imageBytes = await _imagePickerService.ConvertImageToByteArrayAsync(imagePath);
        if (imageBytes != null)
        {
            string imageData = _imagePickerService.ConvertByteArrayToBase64(imageBytes);
            
            var messageWrapper = new MessageWrapper
            {
                Type = MessageType.Image,
                Content = $"📷 Image: {Path.GetFileName(imagePath)}",
                ImageData = imageData,
                Timestamp = DateTime.Now,
                SenderName = _userSelected?.Name ?? "Unknown",
                SenderId = _userSelected?.Id ?? 0
            };

            await _signalingService.SendMessageWrapperAsync(messageWrapper);
        }
    }
}
```

## Message Format

### Text Message JSON
```json
{
  "Type": 0,
  "Content": "Hello Bob! How are you?",
  "ImageData": null,
  "Timestamp": "2025-08-21T22:16:10.3402532+00:00",
  "SenderName": "Alice",
  "SenderId": 1
}
```

### Image Message JSON
```json
{
  "Type": 1,
  "Content": "📷 Image: vacation_photo.jpg",
  "ImageData": "SU1BR0VfREFUQV9GT1JfdmFjYXRpb25fcGhvdG8uanBnXzIwMjUwODIxMjIxNjEw",
  "Timestamp": "2025-08-21T22:16:10.4610155+00:00",
  "SenderName": "Alice",
  "SenderId": 1
}
```

## Current Limitations

1. **Placeholder Image Picker**: The current implementation uses a placeholder for image selection. Real implementation would use `Microsoft.Maui.Essentials.FilePicker`.

2. **No Image Compression**: Images are transmitted as-is. Production implementation should include compression.

3. **Size Limits**: No current limits on image size. WebSocket buffer increased to 4096 bytes but may need further increases for large images.

4. **File Type Validation**: No validation of image file types.

## Future Enhancements

1. **Real File Picker Integration**: Implement `Microsoft.Maui.Essentials.FilePicker` for actual image selection
2. **Image Compression**: Add image compression before transmission
3. **Progress Indicators**: Show upload/download progress for large images  
4. **Thumbnail Generation**: Generate thumbnails for chat display
5. **Image Caching**: Cache images locally to avoid re-downloading
6. **Multiple Image Support**: Allow sending multiple images in one message
7. **Image Viewer**: Full-screen image viewing with zoom capabilities

## Testing

Run the demo console application to see the image message functionality:

```bash
cd TestImageDemo
dotnet run
```

This demonstrates:
- Creating text and image messages
- JSON serialization/deserialization
- Base64 encoding/decoding
- Message type handling

## WebRTC Integration

This image functionality is separate from but complementary to the WebRTC voice/video calling features. The signaling server can handle both chat messages and WebRTC signaling through the same WebSocket connection.
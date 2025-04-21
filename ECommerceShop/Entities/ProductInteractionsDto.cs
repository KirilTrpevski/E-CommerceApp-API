public class ProductInteractionsDto
{
    public Guid InteractionId { get; set; }  // Unique identifier for each interaction
    public Guid UserId { get; set; }  // Foreign key to User
    public Guid ProductId { get; set; }  // Foreign key to Product
    public string InteractionType { get; set; }  // Interaction type ("view", "purchase", etc.)
    public DateTime InteractionDate { get; set; }  // Timestamp of the interaction
}
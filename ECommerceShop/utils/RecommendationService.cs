using ECommerceShop.Data;
using ECommerceShop.Entities;
using Microsoft.EntityFrameworkCore;

public class RecommendationService
{
    private readonly DataContext _context;  // Your database context

    public RecommendationService(DataContext context)
    {
        _context = context;
    }

    // Get all interactions for a particular product
    public async Task<List<ProductInteraction>> GetProductInteractionsAsync(Guid productId)
    {
        return await _context.ProductInteractions
            .Where(pi => pi.ProductId == productId)
            .ToListAsync();
    }

    // Get all products that a user has interacted with
    public async Task<List<Guid>> GetUserInteractedProductsAsync(Guid userId)
    {
        var userIdString = userId.ToString().ToLower();  // Convert userId to lowercase string

        var userProducts = await _context.ProductInteractions
            .Where(pi => pi.UserId.ToString().ToLower() == userIdString)  // Convert UserId to lowercase string in the database
            .Select(pi => pi.ProductId)
            .Distinct()
            .ToListAsync();

        return userProducts;
    }

    // Calculate cosine similarity between two products
    public double CalculateCosineSimilarity(Guid productA, Guid productB)
    {
        var interactionsA = _context.ProductInteractions
            .Where(pi => pi.ProductId == productA)
            .ToList();

        var interactionsB = _context.ProductInteractions
            .Where(pi => pi.ProductId == productB)
            .ToList();

        // Create dictionaries for quick lookup
        var usersA = interactionsA.ToDictionary(pi => pi.UserId, pi => pi.InteractionType);
        var usersB = interactionsB.ToDictionary(pi => pi.UserId, pi => pi.InteractionType);

        // Find common users
        var commonUsers = usersA.Keys.Intersect(usersB.Keys).ToList();


        if (commonUsers.Count == 0)
            return 0; // No common users means no similarity

        // Calculate Cosine Similarity
        double dotProduct = 0;
        double magnitudeA = 0;
        double magnitudeB = 0;

        foreach (var userId in commonUsers)
        {
            var interactionA = usersA[userId] == "Purchase" ? 1 : 0;
            var interactionB = usersB[userId] == "Purchase" ? 1 : 0;

            dotProduct += interactionA * interactionB;
            magnitudeA += interactionA * interactionA;
            magnitudeB += interactionB * interactionB;
        }

        // Ensure we avoid division by zero (check magnitudes)
        if (magnitudeA == 0 || magnitudeB == 0)
            return 0; // Return 0 similarity if one of the vectors has zero magnitude

        // Compute the final cosine similarity
        double similarity = dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));

        return similarity;
    }

    // Recommend products to a user based on item similarity
    public async Task<List<Product>> GetRecommendedProductsAsync(Guid userId)
    {
        var userProducts = await GetUserInteractedProductsAsync(userId);

        var recommendations = new List<Product>();

        foreach (var userProductId in userProducts)
        {
            var products = await _context.Products.ToListAsync();

            foreach (var product in products)
            {
                if (userProductId != product.Id) // Don't recommend the same product
                {
                    var similarity = CalculateCosineSimilarity(userProductId, product.Id);
                    if (similarity > 0.1) // Set your threshold for similarity
                    {
                        recommendations.Add(product);
                    }
                }
            }
        }

        return recommendations;
    }
}

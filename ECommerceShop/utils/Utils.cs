public class PasswordService
{
    // Hash the password
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Verify the password
    public bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
        {
            throw new ArgumentException("Password or stored hash cannot be null or empty");
        }

        return BCrypt.Net.BCrypt.Verify(password, storedHash);  // Verify using the stored hash
    }
}
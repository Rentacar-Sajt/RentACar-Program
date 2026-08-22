using BCrypt.Net;

Console.Write("Unesi administratorsku lozinku: ");

string? lozinka = Console.ReadLine();

if (string.IsNullOrWhiteSpace(lozinka))
{
    Console.WriteLine("Lozinka nije uneta.");
    return;
}

string hash = BCrypt.Net.BCrypt.HashPassword(lozinka);

Console.WriteLine();
Console.WriteLine("Generisani BCrypt hash:");
Console.WriteLine(hash);

Console.WriteLine();
Console.WriteLine(
    "Provera uspešna: " +
    BCrypt.Net.BCrypt.Verify(lozinka, hash)
);

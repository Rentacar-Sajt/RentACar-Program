package Kontroler;

import java.sql.ResultSet;
import java.sql.Statement;

import Model.BazaPodataka;

public class TestDatabaseConnection {
    public static void main(String[] args) {
        try {
            // Proverava da li je Microsoft JDBC drajver dodat u projekat.
            Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
            System.out.println("Microsoft SQL Server JDBC drajver je ucitan.");
        } catch (ClassNotFoundException e) {
            System.out.println("SQL Server JDBC drajver nije pronadjen. Dodaj mssql-jdbc JAR u Build Path.");
            return;
        }

        BazaPodataka bazaPodataka = new BazaPodataka();
        Statement statement = bazaPodataka.getStatement();

        if (statement == null) {
            System.out.println("Konekcija nije uspostavljena.");
            return;
        }

        try (ResultSet rs = statement.executeQuery("SELECT DB_NAME() AS NazivBaze")) {
            if (rs.next()) {
                System.out.println("Povezan si na bazu: " + rs.getString("NazivBaze"));
            }
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            bazaPodataka.close();
        }
    }
}

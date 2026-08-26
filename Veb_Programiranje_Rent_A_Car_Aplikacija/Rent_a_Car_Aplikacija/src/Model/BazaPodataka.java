package Model;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.sql.Statement;

public class BazaPodataka {
    private Connection connection;
    private Statement statement;

    // Podesavanja za SQL Server. Aplikacija koristi Windows Authentication,
    // pa se SQL Server USER i PASSWORD ne cuvaju u kodu.
    // SQL Server se na ovom racunaru u SSMS-u otvara preko localhost.
    private static final String SERVER = "localhost";
    private static final String DATABASE = "rentacarsistem";

    public BazaPodataka() {
        try {
            // Microsoft JDBC konekcioni string za SQL Server sa Windows Authentication.
            // integratedSecurity=true koristi trenutno prijavljen Windows nalog.
            // trustServerCertificate=true je pogodan za lokalni SQL Server tokom razvoja.
            String url = "jdbc:sqlserver://" + SERVER
                    + ";databaseName=" + DATABASE
                    + ";integratedSecurity=true"
                    + ";encrypt=true;trustServerCertificate=true;";

            connection = DriverManager.getConnection(url);
            System.out.println("Uspesno povezivanje sa SQL Server bazom: " + DATABASE);

            statement = connection.createStatement();
            System.out.println("Statement uspesno inicijalizovan.");

        } catch (SQLException e) {
            System.out.println("Greska pri povezivanju sa SQL Server bazom.");
            System.out.println("Proveri da li SQL Server radi, da li je TCP/IP ukljucen, "
                    + "da li je SQL Server dostupan preko localhost i da li Windows nalog ima pristup bazi.");
            e.printStackTrace();
        }
    }

    public Statement getStatement() {
        return statement;
    }

    public Connection getConnection() {
        return connection;
    }

    public void close() {
        try {
            if (statement != null) statement.close();
            if (connection != null) connection.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }
}

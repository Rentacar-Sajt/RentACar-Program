<?php

// Podesavanja za konekciju sa bazom podataka.
return [
    'server' => 'localhost,1433',
    'baza' => 'rentacarsistem_php',
    // Windows autentikacija preko PDO_SQLSRV. Ako PHP radi pod nalogom koji ima pristup SQL Server-u,
    // korisnicko ime i lozinka ostaju prazni.
    'korisnik' => '',
    'lozinka' => '',
];

<?php

// Ruter za REST API. Na osnovu HTTP metode odredjuje sta treba da se uradi.
require_once __DIR__ . '/RestKontroler.php';

class RestRuter
{
    public function obradi(): void
    {
        header('Content-Type: application/json; charset=utf-8');
        // API radi sa standardnim HTTP metodama GET, POST, PUT i DELETE.
        $metoda = strtoupper($_SERVER['REQUEST_METHOD'] ?? 'GET');
        $putanja = trim($_SERVER['PATH_INFO'] ?? '', '/');
        if ($putanja === '') $putanja = trim($_GET['ruta'] ?? '', '/');
        $delovi = $putanja === '' ? [] : explode('/', $putanja);
        $resurs = $delovi[0] ?? '';
        $id = isset($delovi[1]) && ctype_digit($delovi[1]) ? (int)$delovi[1] : null;
        $k = new RestKontroler();

        try {
            if ($resurs === 'iznajmljivanja') {
                if ($metoda === 'GET' && $id === null) { $k->iznajmljivanja(); return; }
                if ($metoda === 'GET' && $id !== null) { $k->iznajmljivanje($id); return; }
                if ($metoda === 'POST' && $id === null) { $k->dodajIznajmljivanje($this->json()); return; }
                if ($metoda === 'PUT' && $id !== null) { $k->izmeniIznajmljivanje($id, $this->json()); return; }
                if ($metoda === 'DELETE' && $id !== null) { $k->obrisiIznajmljivanje($id); return; }
            }
            if ($resurs === 'klijenti' && $metoda === 'GET') { $k->klijenti($id); return; }
            if ($resurs === 'vozila' && $metoda === 'GET') { $k->vozila(); return; }
            $this->odgovor(['greska'=>'Ruta ili HTTP metoda nije podržana.'],404);
        } catch (Throwable $e) {
            $this->odgovor(['greska'=>$e->getMessage()],400);
        }
    }

    private function json(): array
    {
        $sirovo = file_get_contents('php://input');
        $podaci = json_decode($sirovo ?: '{}', true);
        if (!is_array($podaci)) throw new Exception('JSON telo zahteva nije ispravno.');
        return $podaci;
    }

    public static function odgovor(array $podaci, int $status=200): void
    {
        http_response_code($status);
        echo json_encode($podaci, JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
    }
}

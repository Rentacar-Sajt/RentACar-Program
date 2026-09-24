<?php

// Glavna ulazna tacka aplikacije. Sve stranice prolaze kroz ruter.
require_once __DIR__ . '/rutiranje/Ruter.php';
(new Ruter())->obradi();

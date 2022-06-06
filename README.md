# Benchmark
Program Benchmark służy do porównania wybranych baz grafowych. Został napisany w języku C# w technologii WPF. 
Bazy danych zostały wybrane zgodnie z ich popularnością na rynku w roku 2020. Są to: Neo4j, OrientDB, Dgraph, ArangoDB.
Jest to pierwsza, podstawowa wersja programu. 

Program korzysta z danych dostępnych w internecie o największych miastach świata. Pobiera dane z pliku .json i na podstawie szerokości i długości geograficznej
liczy odległości dla każdego z nich tworząc graf. Następnie powstałe dane dodaje do każdej bazy grafowej i na ich podstawie przeprowadza, za pomocą zapytań, 
testy każdej z nich. Wyniki zapisuje, wyciąga wnioski i je przedstawia.  

Główna funkcja programu - testująca i porównująca wyniki znajduje się w pliku /service/DBManager.cs

Obsługa każdej z baz znajduje się w plikach /service/[nazwaBazy].cs

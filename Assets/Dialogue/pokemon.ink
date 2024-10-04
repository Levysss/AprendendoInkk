Olá sou professor Carvalho!

Fico responsavel em destribuir os pokemons inicias.

Espero que você tenha pensado bem!

->main


=== main ===
Qual pokemon você escolhe?
    +[Charmander]
        -> choicen("Charmander")
    +[Bulbasauro]
        -> choicen("Bulbasauro")
    +[Squirtle]
        -> choicen("Squirtle")
=== gostou ===
    +[Sim]
        Ah que otima noticia!
        ->END
    +[Não]
        Ah que pena, então...
        ->main
    

=== choicen(pokemon)===
Você escolheu {pokemon}!
gostou da sua escolha ?
->gostou

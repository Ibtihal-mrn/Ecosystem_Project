# Projet Ecosystème - réalisé par Bikass Imane (22231) et Mournin Ibtihal (22210)

Le but de ce projet est de simuler un écosystème simplifier capable de gérer les interractions entre des animaux carnivores, des herbivores, ainsi que des végétaux. 

Les animaux doivent être capable de se reproduire, de se nourir, de chasser (pour les carnivores), mais également de mourir et de se décomposer. 

Les plantes, quant elles, doivent également être en mesure de se reproduire, de se nourir de déchets organiques, et d'être mangées par des herbivores. 

Pour information, le travail a été séparé en deux. Ibtihal s'est chargé de tout ce qui est lié à l'implémentation du comportement animal, et Imane s'est chargé de l'implémentation du comportement des végétaux. C'est la raison pour laquelle les logiques d'implémentation utilisées ne sont pas toujours les mêmes pour un comportement similaires. Idéalement, il aurait mieux fallu homogénéiser le code, mais faute de temps, nous nous sommes contentées de mettre en commun nos codes. 

## Gestion des points de vie et d'Energie

Afin de gérer le cycle de vie des êtres vivants de l'écosystème, un système a été mis en place basée sur la gestion d'une quantité d'energie initiale, et d'un certains nombre de points de vie. 

Le principe est le suivant : au fur et à mesure que le temps passe, l'energie d'un être vivant baisse. Une fois qu'il n'a plus d'energie, ses points de vie se transformes en énergie, et il finit par mourir une fois qu'il a consommé toutes ses réserves. 

### Plantes 

### Animaux

## Gestion des zones 

Afin de pouvoir gérer les interractions entre les différents êtres vivants, il a fallu définir différentes zones dans lesquelles des actions peuvent avoir lieu. 

Les plantes ont deux zones : la zone de semis et la zone de racine. 

Les animaux ont également deux zones : la zone de contact, et la zone de vision. 

### Plantes 

### Animaux

## Gestion de l'alimentation

Afin de rallonger le cycle de vie d'un être vivant, il faut qu'il puisse gagner en énergie, mais comment ? En étant capable de se nourir, tout simplement. 

Un animal carnivore doit pouvoir chasser pour se nourir d'un herbivore, ou il peut éventuellement manger la viande que les animaux laissent derrière eux une fois mort. Les herbivores, eux, doivent se nourir des plantes. 

Une plante doit pouvoir se nourir des déchets organiques qui proviennent soit de la viande qui s'est décomposée, ou des plantes mortes. 

### Plantes 

### Animaux

## Gestion de la reproduction 

Afin que l'écosystème puisse rester vivant, il est essentiel que les êtres vivants puissent se reproduire. Autrement, une fois qu'ils sont tous morts, il ne reste plus rien et la simulation est terminée. 

### Plantes

### Animaux

## Diagramme UML

## Principes SOLID

## Résultat de la simulation


# **Description du scénario**

Le scénario se place dans un contexte d'une zone sinistrée avec potentiellement plusieurs blessés. L'objectif principal ici est de détecter tous les blessés possible via des drones pour accélérer l'intervention des secours. Des drônes sont alors placés au centre de la zone puis s'envole en se répartissant de manière le plus équitable possible une zone a survoler pour garantir que chaque m2 de la zone a été vérifier. Lorsqu'une personne est détecté par un des drônes celui ci prévient le drône "maître" de la position du blessé et il s'y rends directement. Une fois que le maître est arrivé à la position de la personne , il passe en mode manuel pour permettre aux secours de contrôler le drône et de récupérer toutes les informations dont ils ont besoin via la caméra. Cet essaim offre donc une détection complète de tous les blessés d'une zone sinistrée ainsi qu'une possibilité aux secours d'obtenir des informations sur chacun d'entre eux. Grâce à tout cela , ils peuvent agir rapidement et avec une stratégie parfaitement adapté à la situation.

Drônes utilisé dans l'opération ( le rouge est le drône maître ):

<img src="Images/drones.png">

Une capsule rouge qui fait office de blessé:

<img src="Images/player.png" width="200">

Demonstration du scénario:

![alt text](Images/simulation.gif)

# **Explication du projet**

## **Déroulement de la simulation**

Lorsque la simulation est lancé , il faut appuyer sur la touche "S" comme start pour qu'elle commence. Une fois ceci fait , les drônes s'envolent directement vers l'altitude demandé et ensuite se déplace vers le début de leur zone de surveillance respective , quand ils sont arrivés au début de leur zone , ils commencent à la quadriller et lance la détection des blessés via des raycasts. Lorsqu'un drone détecte une personne , il prévient tous les drones de sa position pour qu'ils aient l'information que cette personne a déjà était reperé et il demande également au drone maitre de venir sur place, les zones sont recalculer avec un drone de moins car le maitre part observer le blessé. Une fois qu'il est arrivé , une vue de caméra se créer pour observer le blessé. Les secours peuvent controler le drone avec les fleches du ```Clavier``` et une fois qu'ils ont finit appuie sur ```Espace``` pour repasser le drone en mode automatique , celui ci retournera alors continuer sa zone de surveillance ou sur le prochain blessé si un ou plusieurs ont été detecté pendant son controle par les secours. Pour finir quand chaque drone a finaliser sa zone , il retourne à son point de décollage excepté le maitre qui lui attends que tous les drones aient finit avant de rentrer pour s'assurer qu'il ne manque pas de blessés. 

## **Les Scripts**

Le projet contient 5 scripts pour mettre en place l'intégralité du scénario, des contrôles des objets à l'interface graphique :

#### Contrôle des drônes :

- **Controller.cs --> Controller** : Ce script permet de gérer tous les drônes présent sur le site du scénario, il peut être comparé à un ordinateur de contrôle au sol. Son rôle est d'assigner aux drônes leur zone de survole et de les recalculer chaque fois que cela est nécessaire.
- **UAV.cs --> UAV** : Il s'agit du script du drône lui même. Il gère les différentes phases de déplacement du drône , monter en altitude , parcourt de sa zone , communication entre les drônes et détection des blessés via des raycast.

#### Interface graphique :

- **Communication.cs --> Communication** : Une zone de texte est présente dans l'UI du scénario sous forme de "tchat" ( des messages s'ecrivent les uns en dessous des autres et lorsque le bord est atteint les nouveaux messages remplacent les premiers ). Ce "tchat" permet de visualiser les communications entre les drônes , par exemple lorsqu'un drône détecte une personne il prévient tous les drônes de sa position (pour qu'il sache que ce blessé a deja été detecté). Ce script permet de gérer cette zone de texte.
- **ProgressBar.cs --> Slider** : Ce script permet simplement de contrôler un slider transformé en barre de progression pour permettre de visualiser directement le pourcentage de la zone déjà survolé. Elle permet de garantir de manière sûr ( la raison sera expliqué plus bas ) que toute la zone a bien été vérifié.

## Répartition intelligentes des zones

L'objectif central de ce scénario est d'offrir une aide précieuse aux secours pour détecter toutes les personnes en détresse dans une zone donnée. Pour accomplir ce but , il faut donc avoir une surveillance de la zone complète à 100% c'est à dire que chaque m2 de celle-ci doit avoir été survolé. 

Pour garantir cette réussite mon projet récupère la longueur et la largeur de l'endroit à survoler , et créer une "map" ( dans le code une liste ) de chaque point par lequel les drônes doivent passer pour avoir tout survolé. Chaque drône est entouré d'un cercle dans le scénario qui corresponds à son champ de détection, en utilisant le diamètre de ce cercle on peut générer des positions chacune espacées de la taille de ce cercle en longueur autant qu'en largeur. On se retrouve alors avec par exemple une zone de 100x100 et des drônes qui on un champ de vision de 4 ce qui fait que l'on aura 25 points en longueur et 25 en largeur ( 100 / 4 = 25).

La méthode expliqué au dessus garantit donc de manière triviale que la zone sinistré sera survolé à 100%. Chaque checkpoints présent dans cette liste sont une partie de la map qu'il faut alors répartir de manière équitable entre les drones. Pour répartir ces checkpoints le nombre de colonne de la map est divisé par le nombre de drones présent lors de la mission et chaque drone se voit assigné ces colonnes excepté le dernier drone qui aura en plus le reste des colonnes. En reprenant l'exemple précedent cela nous donne 25 / 4 donc 6 colonnes par drone et 7 pour le dernier ( 6+6+6+7 = 25).

## Déplacement des drônes dans chaque zone

Comme vu au dessus , les drônes se voit assignés des colonnes de positions de la map de surveillance. Une des subitilités de la génération de la map est que chaque colonne adjacentes sont de sens inverse , c'est à dire que la colonne 0 contient les positions du haut gauche vers le bas gauche et la colonne 1 du bas gauche (+ diametre cercle) vers le haut gauche (+ diametre cercle) etc. Chacune de ses positions est assignés comme une target pour le drone , ce qui fait qu'il va se déplacer en "quadrillage" ( haut-bas bas-haut haut-bas .... ). Une fois que chaque drône n'a plus de positions dans sa liste de target , il a alors terminé le parcours de sa zone est peut alors retourné à sa zone de décollage.

## Détection d'une personne : comportement

Chaque fois qu'un drône détecte un blessé il demande au drone maitre de venir à cette position pour permettre aux secours de voir la situation de cette personne. Mais vu que le drone maitre sort de sa zone de surveillance cela créer un trou dans le survole de la map ( le drône maître n'avance plus dans ses targets ). Pour régler ce problème chaque fois que le drône maitre quitte sa zone , le controller recalcule les zones assignés avec un drône de moins , les 3 autres drônes se voit alors répartir toute la map entre eux. Et pour offrir une optimisation de temps à la mission chaque target deja réalisé par les drones sont stockés par le controller, ce qui fait que les zones réassignés aux 3 drones ne contiennent pas les targets deja survolés. Le fait de stocker les positions deja faites de la map est ce qui permet également de garantir que la barre de progression de survole est sûre car on a au départ une zone découpé à 100% en position , en calculant le pourcentage de targets réalisés par rapport à toutes les targets, on connaît la progression.


## Communications entre les drônes

Comme expliqué brievement plus haut , toutes les communications entre les drônes concernent la détection de personnes. Chaque fois qu'un uav détecte quelque chose , il envoie sa position à tous les autres et en plus "demande" au drône maître de venir sur place. On peut facilement observer ces communications dans l'UI. 

## Présentation du concept des Scènes Unity

Les scènes dans Unity sont les "objets" qui vont contenir tout le contenu du jeu / simulation ... que l'on souhaite créer. Elles vont contenir les joueurs , l'environnement , l'interface graphique etc , et le nombre de scènes dans chaque projet va dépendre des besoins et du niveau de complexité du projet . Il est tout à fait possible de tout combiner dans une seul scène pour un petit jeu 2D par exemple ou faire plusieurs scènes pour un jeu à plusieurs niveaux, chaque scène contenant un niveau avec son environnement , son interface , ses joueurs.

Lors de la création d'une nouveau projet , Unity créer de base une scène contenant une caméra et une lumière. On peut facilement en créer une nouvelle avec ```Clic droit et New Scene```. Pour interagir avec les scènes durant le jeu, il faut connaître les concepts de changement de scène depuis les scripts.

Il existe 2 types de changement de scène :

- Le changement de scène normal : Pour le réaliser il faut utiliser la fontion ```SceneManager.LoadScene(nomDeLaScene)```, il faut savoir qu'étant donné que ce mode change complètement la scène , l'actuelle est détruite pour charger la nouvelle. De ce fait , tout ce qui était dans la scène est détruit. il existe différentes façons de conserver certains objets entre les scènes si les besoins le demande. Il est possible de créer une classe statique qui contiendra les données que l'on veut conserver entre les scènes , il suffira de directement appeler la donnée pour l'utiliser comme ceci : ```MaClasse.MaDonnée```, cela ne fonctionne que pour les données de type simple comme les int , string etc pas pour les objets. Pour ceux ci, il faut utiliser la fonction ```DontDestroyOnLoad(MonObjet)``` qui va permettre d'empêcher la destruction de l'objet, il se retrouvera présent dans la nouvelle scène.
- Le changement de scène dit "additive" : Contrairement au précèdent ce changement de scène ne détruit pas la scène courante mais ajoute la nouvelle par dessus l'autre , l'utilisation de contenu entre les scènes est alors simple mais ceux ci se superposent , étant donné que la scène se met par dessus l'autre on peut voir la première scène en fond.

## Sources

https://assetstore.unity.com/packages/3d/vegetation/environment-pack-free-forest-sample-168396 : Lien asset store que j'ai utilisé pour les arbres et les pierres
https://assetstore.unity.com/packages/3d/vehicles/air/simple-drone-190684 : Lien asset store que j'ai utilisé pour les drones
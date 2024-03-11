# TEST_chargerDLLRuntime

Projet d'exemple.
- Utilisation par un programme de **librairies** externes au format `.dll`. 
- **Événements de *build***, instructions **Batch**.
- Dépendances d'un projet à un autre.
- Exploration de type.

---

## Exemple 1 : librairie tierce externe

Le projet `DllExterne` est de type **Bibliothèque de classe**, c'est-à-dire qu'il sert à générer une librairie. Le fichier `.dll` est produit en mode `Debug` au chemin par défaut `bin\Debug\net8.0` ; c'est ce chemin qui est utilisé par le projet `App` pour charger la librairie et l'utiliser. Le projet `App` est de type **Application console**. On imagine que cette application ne connaît pas la librairie, ce qui implique explorer la structure de cette dernière. Voir le type `App.Exemple1`.

---

## Exemple 2 : librairie module dépendante du programme

### Présentation

Le projet `App` contient un répertoire `Modules`. Ce répertoire accueille un fichier `.dll` généré par un projet spécifique `TrollTolkien`. Ce projet spécifique utilise un type déclaré dans `App` : le type `Troll`. On dispose donc de modules qui déclarent des trolls spécifiques à partir d'un type général de troll déclaré dans le programme principal.

Le type `Troll` est `abstract`. La librairie `TrollTolkien` propose des types enfants. `Troll` est `public`, sans quoi il n'est pas accessible par un autre projet.

### Répertoire des modules

Le répertoire `Modules` de `App` doit être généré à la compilation. On crée donc ce dossier dans la solution. Or, un dossier vide n'est pas généré en sortie par Visual Studio. Pour générer ce dossier, il lui faut au moins un fichier. Donc, un fichier texte dit **mannequin** (*dummy* en anglais) est créé dans la solution, avec la propriété `Copier dans le répertoire de sortie` à la valeur `Copier si plus récent`... puis ce fichier est supprimé à la compilation grâce à des **commandes Batch/PowerShell** d'**événement post-build** (voir les propriétés du projet pour la configuration) : 

```Batch
echo "Supprimer le fichier Modules\Aide.txt mais conserver le dossier"
cd "$(OutDir)\Modules\"
del "Aide.txt"
```

On peut générer le dossier `Module` d'une autre manière, cette fois juste avant la compilation, en commandes Batch d'**événement pré-build**. C'est la solution utilisée dans le présent projet.

```
if not exist "$(TargetDir)Modules" mkdir "$(TargetDir)Modules"
```

Ecrire les commandes directement dans la fenêtre de gestion de projet a pour effet de mettre à jour le fichier `.csproj`.

Sources :
- MS Learn : https://learn.microsoft.com/fr-fr/visualstudio/ide/how-to-specify-build-events-csharp
- Aide des commandes : https://www.tutorialspoint.com/batch_script/batch_script_commands.htm

Le présent projet entend illustrer l'utilisation des événements de build. En dehors de cette illustration, le programme doit aussi gérer la présence du dossier des modules, les librairies tierces, etc. Exemple : gérer le cas où le dossier n'existe pas au lancement du programme. Ceci n'est pas présenté ici pour ne pas alourdir les scripts et la présentation. 

### Librairie module

On décide que **l'édition des modules s'effectue dans la présente solution**.

Le projet `TrollTolkien` est de type **Bibliothèque, librairie DLL**. On veut le créer dans la solution. Or, plusieurs problèmes se posent.
- Le projet est alors au même plan que le projet principal. Pour expliciter que c'est un module, on préfèrerait le placer dans un dossier `Modules` **à la racine de la solution** (ne pas confondre avec le dossier idoine de `App` qui va accueillir le fichier `.dll`)...
- ... Or, générer le dossier depuis l'**Explorateur de solutions** ne génère pas le dossier correspondant dans le disque dur. C'est simplement une vue commode pour le développement dans Visual Studio. Faisons cela néanmoins...
- ... Donc, en plus d'avoir créé le dossier dans l'**Explorateur de solutions**, on crée maintenant manuellement le dossier `Modules` depuis l'**Explorateur de fichier Windows**, à la racine de la solution...
- ... Maintenant, créons le projet `TrollTolkien` par un clic droit sur le dossier `Modules` de l'**Explorateur de solutions**. Prenons soin aussi de placer physiquement ce nouveau projet dans le répertoire `Modules` situé à la racine de la solution. Ainsi, **vue Visual Studio et répertoires locaux ont la même structure**.

### Compiler

La compilation de `App` n'exclut par les modules, c'est-à-dire qu'à la compilation l'application et chaque module sont compilés selon leur sortie respective, par défaut chacun dans son dossier `\bin`. 

On pourrait néanmoins exclure `TrollsTolkien` de la compilation de la solution.
- Clic droit sur la solution dans l'**Explorateur de solutions** puis choisir `Propriétés`.
- Sélectionner l'onglet `Propriétés de configuration`.
- Décocher la case `Générer` aux lignes du projet `TrollTolkien`, pour les modes *Debug* et *Release*.

Pour compiler un module :
- depuis l'**Explorateur de solutions**, clic droit sur le projet module puis choisir `Générer`,
- ou bien depuis le menu `Générer`, choisir `Générer TrollTolkien`.

### Module dépendant

Le module **dépend** de `App` qui représente notre programme principal. Cette dépendance se déclare. 
- Clic droit sur le projet dans l'**Explorateur de solutions**, puis choisir `Ajouter > Référence de projet...`.
- Cocher la case en regard du projet duquel dépendre.
- Dorénavant, la compilation du projet module génère **aussi** les dépendances qui lui sont nécessaires. Elles ne nous sont pas utiles puisque nous ne manipulons que le fichier `.dll`.

### Editer le module

Le module présente un dossier/*namespace* `Trolls` qui contient les différents types héritant du type `App.Troll`. Attention, pour que ces types soient accessibles de l'extérieur (depuis `App` par exemple), ils doivent être déclarés en `public`.

Le type `TrollSauron` présente des membres supplémentaires à des fins de test.

### Module : événements de build

Nous voudrions copier le fichier `.dll` de sortie de module dans le répertoire `Modules` du dossier de sortie de `App`. Résultat attendu : 

```
App\Modules\TrollTolkien.dll
```

On va utiliser les événements post-build. Pour copier le fichier, par exemple en prenant en compte les modes `Debug` et `Release` :

```
goto :$(ConfigurationName)

:Debug
echo "Débogage"
copy "$(TargetPath)" "..\..\App\bin\Debug\net8.0\Modules\$(TargetFileName)"
goto :exit

:Release
echo "Production"
echo "TO DO"
goto :exit

:exit
```

### Ordre de la compilation

Si la solution est configurée pour compiler tous les projets en même temps, alors rien de particulier n'est à relever.

Si les modules sont exclus de la compilation alors compiler un module avant le projet principal n'est pas possible puisqu'il manque les ressources de ce projet principal. Par conséquent :
1. d'abord compiler le projet principal, puis compiler le module,
2. ensuite, on peut mettre à jour un module ou le programme principal librement.
   - Module : l'arborescence de sortie du programme principal est disponible, les commandes post-build de module remplacent le fichier `.dll` précédent par le plus récent.
   - Programme principal : le dossier `Modules` de sortie est conservé en l'état s'il existe déjà, ce qui laisse son contenu inchangé (les modules existants ne sont donc pas détruits).

### Exploration de la librairie

Le type `App.Exemple2` présente une exploration du type `TrollSauron` du module avec `System.Reflection`.

# Chuegelibahn
A VR Chügelibahn Simulator and Editor for _3D User Interfaces & Experience Design_


## Grobkonzept

### Idee

Wir wollen für VR oder AR (noch nicht definitiv entschieden) einen einfachen Kugelbahn Simulator erstellen. Der Kugelbahn Simulator soll einen eigenen Editor haben, in dem verschiden Elemente zu einer Kugelbahn zusammengebaut werden können. Diese kann dann getestet werden. Der Hauptfokus liegt auf dem Editor, und auf dessen Intuitivität. 


### Objekt-Interaktion & Navigation

Alle Bauteile der Kugelbahn können erschaffen, bewegt, (fix) platziert und gelöscht werden.
Bauteile können frei hingestellt (und der Physik überlassen), aneinander fixiert, oder einfach statisch fix in der Welt positioniert werden.
Die Bauteile können gestreckt, vergrössert, verkleinert werden. Beispielsweise gibt es nur ein Objekt "Gerade Röhre", die kann aber in beliebige verschiedene Längen gezogen werden. Ob die Krümmung auch Teil von den Interaktionsmöglichkeiten an einem Objekt sein wird, oder ob es Teile jeweils in einer geraden und in gekrümmten Versionen geben wird, ist noch nicht definiert. Es muss zuerst getestet werden was davon intuitiver ist, und welche technischen Challenges das krümmen stellen wird.


### UI Challenges

Die Challenge liegt darin, den Editor möglichst intuitiv bedienbar zu gestalten. Das Ziel ist, dass möglichst keine Erklärungen der Interaktionsmöglichkeiten nötig sind. Das greiffen und bewegen von Bauteilen wird dank der Natur von VR vermutlich eher einfach zu gestalten. Folgende Interaktionen können möglicherweise eine grössere UI Challenge darstellen:
- Skalieren von Objekten
- Neue Objekte generieren
- Exaktes platzieren von Objekten


### Software and Hardware Assets

#### Hardware
- VR Headset und Controller.

#### Software
- Unity3D
- 3D Objekte werden von uns erstellt, oder stammen aus Open Source Libraries.

#### 3D Objekte
- Bauteile: Kleinere Objekte, wie geschlossene Röhren, offene Röhren, Klötze, etc.
- Möbel: Fixe grössere Objekte, auf und um diese herum gebaut werden kann.
- Kugeln: Diverse Kugeln, die nach physikalischen Gesetzen die Bahn herunterrollen.


### Weitere Überlegungen

- Objekte, die weder gehalten noch fixiert werden, bewegen sich nach physikalischen Gesetzen.
- Evtl kann die Bahn später (am Computer) mit einer Kamera in einer durchsichtigen Kugel selbst "erfahren" werden
- Objekte könnten aufleuchten, um zu zeigen, welches man im Moment mit einer greifbewegung auswählen würde.

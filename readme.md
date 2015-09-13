#Electric Network Toolkit (ElecNetKit)
_A software library for answering questions with electrical network simulations._

ElecNetKit is a toolkit for building simulation experiments for electrical network models.

There are plenty of good electrical network simulators out there, but most don't really facilitate experimentation or control algorithm development. It's often difficult to move through network topologies, difficult to predict the effect of a change, and overall, difficult to conceptualise cause-and-effect relationships. As a particular example, consider the effect of widespread [distributed generation](http://en.wikipedia.org/wiki/Distributed_generation) on the electrical network. It's something that's been studied a lot, but typically the pattern using traditional simulator software is:

- Choose a level of penetration
- Manually adjust all the generators
- Check the results manually, or watch for standards violation alerts.

That specific example is a problem I had with my [undergraduate thesis](http://capnfabs.net/static/thesis), and so I set out to develop a new paradigm for understanding and making decisions using electrical network data.

ElecNetKit is implemented as a software library, using the Microsoft .NET framework, which means it's interoperable, powerful and simple to use.

#Installing#
You can grab ElecNetKit from the source here at github, or you can get binaries from [NuGet](http://nuget.org/packages?q=ElecNetKit).

#Documentation#
The [ElecNetKit documentation](http://elecnetkit.github.io/ElecNetKit/) contains a number of conceptual and [Getting Started](http://elecnetkit.github.io/ElecNetKit/?topic=html/9e5ced0a-ca06-45d2-b65e-27c75e679471.htm) topics that you may find helpful. In addition, the docs contains an extensive API documentation. Documentation is also available as a [Compiled HTML Help File](http://elecnetkit.github.io/ElecNetKit/ElecNetKitDocs.chm) if you're into that kind of thing.

#Licence and Contrib#
Licensed under the [Apache 2 License](http://www.apache.org/licenses/LICENSE-2.0).

Contributions are always welcome, particularly if you've got a favourite simulator for which you'd like to add support :)

#Acknowledgements
ElecNetKit was started as part of an [undergraduate thesis project](http://capnfabs.net/static/thesis) and with sponsorship from the [Endeavour Energy Power Quality & Reliability Centre](http://www.elec.uow.edu.au/eepqrc/) at the [University of Wollongong](http://www.uow.edu.au). Thanks also to [Phil Ciufo](http://www.uow.edu.au/~/ciufo/) for his input.

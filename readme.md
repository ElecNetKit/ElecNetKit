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
You can grab ElecNetKit from the source here at github, or you can get binaries from NuGet.

#Documentation#
Documentation is available at ...

#Licence#
Licensed under the [Apache 2 License](http://www.apache.org/licenses/LICENSE-2.0).
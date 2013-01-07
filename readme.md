#Electric Network Toolkit (ElecNetKit)
_A software library for answering questions with electrical network simulations._

ElecNetKit is a toolkit for building simulation experiments for electrical network models.

There are plenty of good electrical network simulators out there, but most don't really facilitate experimentation or control algorithm development. It's often difficult to draw relationships using network topologies, difficult to predict the effect of a change, and overall, difficult to conceptualise cause-and-effect relationships. As a particular example, consider the effect of widespread [distributed generation](http://en.wikipedia.org/wiki/Distributed_generation) on the electrical network. It's something that's been studied a lot, but typically the pattern using traditional simulator software is:

- Choose a level of penetration
- Manually adjust all generators
- Check the results manually, or watch for standards violation alerts.

That specific example is a problem I had with my [undergraduate thesis](http://capnfabs.net/static/thesis), and so I set out to develop a new paradigm for understanding and making decisions using electrical network data.

ElecNetKit is implemented as a software library, using the Microsoft .NET framework, which means it's highly interoperable, and simple and expressive to use.

##Experimentation and Analysis##

ElecNetKit is founded on an architecture comprised of **experiments** and **transforms**.

An **experiment** is a code module that uses a network topology to generate a set of simulator instructions. These instructions modify the circuit.

A **transform** is a module that hooks into the experimentation process, and gets a peek at the network before and after experimentation. Results can be compared, analysed and transformed, enabling complex analysis, use of real-time data, and control-oriented decision making.
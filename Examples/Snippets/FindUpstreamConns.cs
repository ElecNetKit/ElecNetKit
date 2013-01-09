Bus myBus = Circuit.Buses["myBus"];
//Find all connected lines where:
myBus.ConnectedTo.OfType<Line>().Where(
  //the other bus the line connects to
  line => line.ConnectedTo.OfType<Bus>().Where(x => x != myBus)
  .First() //Convert from list to single element
  //check to see if the connected bus has a higher voltage than myBus.
  .Voltage.Magnitude > myBus.Voltage.Magnitude 
);
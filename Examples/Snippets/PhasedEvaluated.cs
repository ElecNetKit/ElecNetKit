public class Bus
{
	public Phased<Complex> VoltagePhased { private set; get; }

	public double BaseVoltage { set; get; }

	public Phased<Complex> VoltagePUPhased { private set; get; }

	public Bus() // or OnDeserializationCallback, whatever
	{
		VoltagePUPhased = new PhasedEvaluated<Complex,Complex>(
                from => from / this.BaseVoltage, //get
                to => to * this.BaseVoltage, //set
                VoltagePhased
                );
	}
}
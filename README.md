# EvictPowerSrv

Shutdown the system properly before Azure evicts your Spot instance.

## Background

Azure Spot instance is a great place for Lab environments, which greatly reduces cost with no SLA guaranteed. However, when VMs get evicted, they are not shutdown gracefully, but rather "hard". Thus making data unstable.

The only chance to receive shutdown notification is through Scheduled Events. We have a 30s notification before cutting the power, so that's how the project works.

The project is a Windows service continually polling the latest events, and shutdown the system immediately before the power is cut.

## Disclaimer

This is my first C# project, and I'm not quite familiar with C#, so any suggestions would be appreciated. 

## Usage

Download the latest MSI, install, and you're done.

## License

GPL v2 only.
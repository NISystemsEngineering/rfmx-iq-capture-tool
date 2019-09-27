# RFmx IQ Capture Tool
A simple command line tool to fetch raw IQ data from RFmx measurement configurations.

# Usage
1) Configure your measurements as desired in the RFmx Soft Front Panel (SFP) or via the API
2) Export the measurement configuration to disk from the SFP (see screenshot below) or via the API 
![RFmxSFP](/img/SaveConfigurationRFmx.png)
3) Call `RFmxIQCaptureTool` via the flags below ![CaptureTool](/img/CommandLineUsage.png)
4) Follow the instructions in the command prompt window to run the measurements configured in the file and save them to disk

## Known Issues
- Measurements that use spectral acquisition mode are not supported

# Questions/Issues

Please report all questions and issues via the [Issues](https://github.com/NISystemsEngineering/rfmx-iq-capture-tool/issues) section.

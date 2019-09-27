# RFmx IQ Capture Tool
A simple command line tool to fetch raw IQ data from RFmx measurement configurations.

# Process
1) Configure your measurements as desired in the RFmx Soft Front Panel (SFP) or via the API
2) Export the measurement configuration to disk from the SFP (see screenshot below) or via the API 
![RFmxSFP](/img/SaveConfigurationRFmx.png)
3) Call the **RFmxIQCaptureTool**; see the usage instructions below.
4) Follow the instructions in the command prompt window to run the measurements configured in the file and save them to disk
![Usage](/img/CommandLineUsage.png)
5) One file will be saved per signal configured in the TDMS configuration file that you choose to acquire. The data will be saved as interleaved IQ data in a text file.

# Usage
- Output IQ data in the working directory for an RFmx configuration named *SampleConfig.tdms* and an instrument named *5840*:

    `RFmxIQCaptureTool --path SampleConfig.tdms --instr 5840`
  
- Output IQ data to *..\Files* for an RFmx configuration named *SampleConfig.tdms* and an instrument named *5840*:
  
    `RFmxIQCaptureTool --path SampleConfig.tdms --instr 5840 --output ..\Files`

### Arguments
  `-i, --instr`     **Required**. The instrument name to use for acquisition.

  `-f, --path`      **Required**. The TDMS file path defining the RFmx configuration.

  `-o, --output`    The output folder for the IQ data.

  `--help`          Display the help screen.

  `--version`       Display version information.

## Known Issues
- Measurements that use spectral acquisition mode are not supported

# Questions/Issues

Please report all questions and issues via the [Issues](https://github.com/NISystemsEngineering/rfmx-iq-capture-tool/issues) section.

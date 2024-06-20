<div align="center">

<img src="https://github.com/minisbett/holdtimeanalyzer/assets/39670899/76878cdf-26e0-47b4-b44e-f184a118a5cb" width="192">

# Hold-Time Analyzer

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Downloads](https://img.shields.io/github/downloads/minisbett/holdtimeanalyzer/total?style=flat&color=40b86b
)](https://github.com/minisbett/holdtimeanalyzer/releases/latest)
[![Latest Release](https://img.shields.io/github/v/release/minisbett/holdtimeanalyzer?color=ff5867
)](https://github.com/minisbett/holdtimeanalyzer/releases/latest)

An easy-to-use bulk hold-time analyzer, generating graphs for the hold times of osu!standard replays.

[Usage](#usage) • [Development](#development) • [Demo](#demo)<br/>

<i>Made with ❤️ by @minisbett</i>
</div>

## Usage

To get started, head over to the [latest release page](https://github.com/minisbett/holdtimeanalyzer/releases/latest) on GitHub. From there, locate and download the ZIP file (<u>ending with `.zip`</u>). Once the download is complete, extract the ZIP file.

On the first start of the application, a `replays` folder will be created in the same directory as the `.exe` file.

Here's a step-by step instruction on how to use the hold-time analyzer:
1. Place all all replays you'd like to generate a graph for in the `replays` folder.
2. Launch the application. It will process all replays, and report whether processing was successful or, in rare cases, was skipped.
3. Get the resulting PNG files containing the graph from the `output` folder, which can also be found in the same directory as the `holdtimeanalyzer.exe` file.

**Note:** The PNG files will have the same name as the replay files used to generate them. (eg. `replay1.osr` becomes `replay1.png`)

---

If an error occured, the application will report it to you in the console. Additionally, a detailed error report TXT file is created in the `output` folder, with the same name as the replay that caused the error. In this case, feel free to report it in the [issues section](https://github.com/minisbett/holdtimeanalyzer/issues) of this repository.

## Development

If you'd like to suggest new features and/or changes for this project, please open an [issue](https://github.com/minisbett/holdtimeanalyzer/issues) first so that we can discuss it.

## Demo

![4629766969](https://github.com/minisbett/holdtimeanalyzer/assets/39670899/ea58e559-d48b-449e-bf73-973f89c4d8ea)

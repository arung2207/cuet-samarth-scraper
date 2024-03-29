# CUET (UG) - 2024 - Data Scraper
Fetches list of university programmes (along with the eligibility criterias) from all the universities listed on the [CUET (UG) - 2024 Website](https://cuetug.ntaonline.in/universities/), into a CSV file. Data updated as of 12th Mar 12.30p IST can be found [here](https://github.com/arung2207/cuet-samarth-scraper/blob/main/cuetdata%20-%202024.csv).

![image](https://github.com/arung2207/cuet-samarth-scraper/assets/3456937/77d7a9a5-525f-4e51-897e-305dcba09c56)

The extracted list can be used to shortlist the options by setting filters in a CSV reader such as Microsoft Excel.

![image](https://github.com/arung2207/cuet-samarth-scraper/assets/3456937/7ab968c7-c852-475b-84cc-252c7be6d19f)

The list of programmes is evolving and changes almost daily. To see the delta differences between any two days compare the exported files using a file diff tool such as [WinMerge](https://winmerge.org/). The differences show up as follows:

![image](https://github.com/arung2207/cuet-samarth-scraper/assets/3456937/0dd92d84-36b9-4303-bedb-9e054f9e6bd6)

Tools used: .Net 6.0 Console Application, HTTPClient, HtmlAgilityPack, Fizzler, CSVHelper.
To edit or execute: Download the source code and open using [Microsoft Visual Studio Community 2022](https://visualstudio.microsoft.com/vs/community/)
To view the latest export: Download the CSV file from the link given above.

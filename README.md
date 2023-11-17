# nvmParser
app to parse and view NVM data
initially it was app to visualize Non Volatile Memory data
It uses ruXaml.xaml file to define data structure to be found and filled from binary stream.
It is two stage process:
1. to load data description from ruXaml.xaml file (any xaml file later)
2. to parse data stream from C# array (from binary file later)

As result you should have binary stream presented as set of structures in tree view.

You can define your own structure description in your own Xaml file and so visualize your binary data in the way that supposed to be easier than looking into HEX presentation of the stream.

falak.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs ParserTest.cs \
	SyntaxError.cs Node.cs SpecificNodesTest.cs

	mcs -out:falak.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	ParserTest.cs SyntaxError.cs Node.cs SpecificNodesTest.cs

clean:

	rm -f falak.exe
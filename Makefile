falak.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs ParserTest.cs \
	SyntaxError.cs Node.cs SpecificNodesTest.cs FirstVisitor.cs \
	SemanticError.cs Type.cs LocalTable.cs GlobalVarTable.cs

	mcs -out:falak.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	ParserTest.cs SyntaxError.cs Node.cs SpecificNodesTest.cs FirstVisitor.cs \
	SemanticError.cs Type.cs LocalTable.cs GlobalVarTable.cs

clean:

	rm -f falak.exe
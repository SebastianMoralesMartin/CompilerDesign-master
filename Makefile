falak.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs ParserTest.cs \
	SyntaxError.cs Node.cs SpecificNodesTest.cs SecondVisitor.cs FirstVisitor.cs \
	SemanticError.cs Type.cs LocalTable.cs GlobalVarTable.cs

	mcs -debug -out:falak.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	ParserTest.cs SyntaxError.cs Node.cs SpecificNodesTest.cs SecondVisitor.cs FirstVisitor.cs \
	SemanticError.cs Type.cs LocalTable.cs GlobalVarTable.cs

clean:

	rm -f falak.exe
falak.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs ParserTest.cs \
           	SyntaxError.cs Node.cs SpecificNodesTest.cs SemanticVisitor.cs \
           	Type.cs SemanticError.cs
           
           	mcs -out:falak.exe Driver.cs Scanner.cs Token.cs TokenCategory.cs \
           	ParserTest.cs SyntaxError.cs Node.cs SpecificNodesTest.cs SemanticVisitor.cs \
           	Type.cs SemanticError.cs
clean:

	rm -f falak.exe
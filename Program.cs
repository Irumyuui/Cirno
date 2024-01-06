using System;
using Cirno.AbstractSyntaxTree;
using Cirno.DiagnosticTools;
using Cirno.Lexer;
using Cirno.Parser;

string[] lines =
[
    "int x[];",
     "int minloc(int a[], int low, int high) {",
     "    int i; int k;",
     "    k = low; x = a[low];",
     "    while (i < high) {",
     "           int xx;",
     "        if (a[i] < x) {",
     "            x = a[i]; k = i;",
     "        }",
     "        i = i + 1;",
     "    }",
     "    return k;",
     "}",
    "int main(void) {",
    "  return 0;",
    "}",
];

var lexer = new Lexer(lines);
var tokens = lexer.GetTokens();
if (lexer.Diagnostics.Count > 0)
{
    PrintDiagnostics(lexer.Diagnostics);
    return;
}

var parser = new Parser(tokens, lexer.Diagnostics);
var exprTree = parser.Parse();
exprTree.Dump();

if (parser.Diagnostics.Count > 0)
{
    PrintDiagnostics(parser.Diagnostics);
    return;
}

var astTree = new AST(exprTree);
astTree.Dump();

return;

void PrintDiagnostics(DiagnosticList diagnosticList)
{
    foreach (var item in diagnosticList)
    {
        Console.WriteLine(item);
    }
}

// string[] lines = [
//     // "int main(void) {",
//     // " int a; int b[10]; int c;",
//     // " a = 10;",
//     // // " b = a + 20;",
//     // " c = 1 + (10 == 2);",
//     // " return 0;",
//     // "}",
//     // "/*adadw*/",
//     // "int x[10];",
//     // "int minloc(int a[], int low, int high) {",
//     // "int i; int x; int k;",
//     // "k = low;",
//     // "x = a[low];",
//     // "while (i < high) {",
//     // "if (a[i] < x) {x = a[i]; k = i;} i = i + 1;",
//     // "}",
//     // "return k;",
//     // "}"
//     "int x[];",
//     "int minloc(int a[], int low, int high) {",
//     "    int i; int k;",
//     "    k = low; x = a[low];",
//     "    while (i < high) {",
//     "           int xx;",
//     "        if (a[i] < x) {",
//     "            x = a[i]; k = i;",
//     "        }",
//     "        i = i + 1;",
//     "    }",
//     "    return k;",
//     "}"
// ];
//
// var tree = new ExpressionTree(lines);
// tree.Dump();
//
// var astTree = new AST(tree);
//
// ASTNode.Dump(astTree.Root);
//
// DiagnosticHelper.PrintDiagnostics();

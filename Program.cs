using System;
using Cirno.AbstractSyntaxTree;
using Cirno.CodeGen;
using Cirno.DiagnosticTools;
using Cirno.Lexer;
using Cirno.Parser;

//string[] lines =
//[
//    "int x[1];",
//     "int minloc(int a[], int low, int high) {",
//     "    int i; int k; int x;",
//     "    k = low; x = a[low];",
//     "    while (i < high) {",
//     "           int xx;",
//     "        if (a[i] < x) {",
//     "            x = a[i]; k = i;",
//     "        } else {",
//    "           k = 2;",
//    "         }",
//     "        i = i + 1 * 2;",
//     "    }",
//     "    return k;",
//     "}",
//    "int main(void) {",
//    "  return minloc(x, low, high);",
//    "}",
//];

string[] lines =
[
    "int level;",
    "int gcd(int u, int v) {",
    "  if (v == 0) {",
    "    return u;",
    "  } else {",
    "    return gcd(v, u - u / v * v);",
    "  }",
    "}",
    "",
    "void main(void) {",
    "  int u;",
    "  int v;",
    "  u = input();",
    "  v = input();",
    "  level = 0;",
    "  output(gcd(u, v));",
    "}",
];

// string[] lines =
// [
//     "/* A program to perform selection sort on a 10",
//  "element array. */",
//
//     "int x[10];",
// "",
//     "int minloc(int a, int low, int high)",
//     "{",
//     "int i; int x; int k;",
//     "k = low;",
//     "x = a[low];",
// "",
//     "i = low + 1;",
// "",
//     "while (i < high) {",
//     "if (a[i] < x) {",
//     "x = a[i];",
//     "k = i;",
//     "}",
//     "i = i + 1;",
//     "}",
// "",
//     "return k;",
//     "}",
// "",
//     "void sort(int a, int low, int high)",
//     "{",
//     "int i; int k;",
//     "i = low;",
//     "while (i < high - 1) {",
//     "int t;",
//     "k = minloc(a, i, high);",
//     "t = a[k];",
//     "a[k] = a[i];",
//     "a[i] = t;",
//     "i = i + 1;",
//     "}",
//     "}",
// "",
//     "void main(void)",
//     "{",
//     "int i;",
//     "i = 0;",
//     "while (i < 10) {",
//     "x[i] = input();",
//     "i = i + 1;",
//     "}",
// "",
//     "sort(1, 0, 10);",
//     "i = 0;",
//     "while (i < 10) {",
//     "output(x[i]);",
//     "i = i + 1;",
//     "}",
//     "}",
//
// ];

// string[] lines =
// [
//     "int foo(int x, int i) {",
//     " return x[i];",
//     "}",
//     "void main(void) {",
//     " foo(1, 2);",
//     "}"
// ];

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

using var visitor = new CodeGenVisitor("main", parser.Diagnostics);
astTree.Root.Accept(visitor);

// PrintDiagnostics(visitor.Diagnostics);

visitor.Diagnostics.Dump(lines);

visitor.Dump();

visitor.Verify(out var message);

Console.WriteLine(message);

await visitor.CompileIR2ExeFile("main.out");

//DiagnosticList.PrintDiagnostics(visitor.Diagnostics);



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

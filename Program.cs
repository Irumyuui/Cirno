using System;
using System.IO;
using Cirno;
using Cirno.AbstractSyntaxTree;
using Cirno.Diagnostic;
using Cirno.Diagnostic;
using Cirno.Expressions;

string[] lines = [
    // "int main(void) {",
    // " int a; int b[10]; int c;",
    // " a = 10;",
    // // " b = a + 20;",
    // " c = 1 + (10 == 2);",
    // " return 0;",
    // "}",
    // "/*adadw*/",
    // "int x[10];",
    // "int minloc(int a[], int low, int high) {",
    // "int i; int x; int k;",
    // "k = low;",
    // "x = a[low];",
    // "while (i < high) {",
    // "if (a[i] < x) {x = a[i]; k = i;} i = i + 1;",
    // "}",
    // "return k;",
    // "}"
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
    "}"
];

// var lexer = new Cirno.Lexer(lines);

// SyntaxToken token;
// do {
//     token = lexer.NextToken();
//     Console.WriteLine(token);
// } while (token.Kind is not SyntaxKind.EndOfFile);

// var lines = File.ReadAllLines("a.c");

var tree = new ExpressionTree(lines);
tree.PrettyPrint();

var astTree = new AST(tree);

ASTNode.PrettyPrint(astTree.Root);

DiagnosticHelper.PrintDiagnostics();

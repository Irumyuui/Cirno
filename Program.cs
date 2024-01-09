using System;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;

namespace Cirno;

internal sealed class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var rootCommand = new System.CommandLine.RootCommand(
            "一个基于LLVMSharp实现的C-编译器前端"
        );

        var inputFilePathOpt 
            = new System.CommandLine.Option<string?>(
                name: "--file", description: "输入的c-文件"
            )
            {
                IsRequired = true
            };
        inputFilePathOpt.AddValidator(result =>
        {
            var filePath = result.GetValueForOption(inputFilePathOpt);
            if (!System.IO.File.Exists(filePath))
            {
                result.ErrorMessage = $"无效的输入文件路径: {filePath}";
            }
        });
        rootCommand.AddOption(inputFilePathOpt);
        // rootCommand.SetHandler(file => );

        var outputFilePathOpt
            = new System.CommandLine.Option<string?>(
                name: "--output", description: "输出文件地址"
            )
            {
                IsRequired = true
            };
        outputFilePathOpt.AddValidator(result =>
        {
            var filePath = result.GetValueForOption(outputFilePathOpt);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                result.ErrorMessage = $"无效的输出文件路径: {filePath}";
            }
        });
        rootCommand.AddOption(outputFilePathOpt);

        var emit2IrOpt = new System.CommandLine.Option<bool>(
            name: "--emit-llvm",
            description: "输出LLVM IR"
        )
        {
            IsRequired = false
        };
        rootCommand.AddOption(emit2IrOpt);

        var dumpExprOpt = new System.CommandLine.Option<bool>(
            name: "--dump-expr",
            description: "输出语法树"
        )
        {
            IsRequired = false
        };
        rootCommand.AddOption(dumpExprOpt);
        
        var dumpAstOpt = new System.CommandLine.Option<bool>(
            name: "--dump-ast",
            description: "输出抽象语法树"
        )
        {
            IsRequired = false
        };
        rootCommand.AddOption(dumpAstOpt);

        var dumpTokensOpt = new System.CommandLine.Option<bool>(
            name: "--dump-tokens",
            description: "输出记号流"
        )
        {
            IsRequired = false
        };
        rootCommand.AddOption(dumpTokensOpt);

        rootCommand.SetHandler(async (inputFilePath, outputFilePath, isEmitIr, isDumpExpr, isDumpAst, isDumpTokens) =>
            await StartSolution(inputFilePath!, outputFilePath!, isEmitIr, isDumpExpr, isDumpAst, isDumpTokens),
            inputFilePathOpt, 
            outputFilePathOpt, 
            emit2IrOpt,
            dumpExprOpt,
            dumpAstOpt,
            dumpTokensOpt
        );
        
        return await rootCommand.InvokeAsync(args);
    }

    internal static string[] ReadFile(string file)
    {
        return System.IO.File.ReadLines(file).Select(line => $"{line} ").ToArray();
    }

    internal static async Task StartSolution(string inputFilePath, string outputFilePath, bool isEmitIr, bool isDumpExpr, bool isDumpAst, bool isDumpTokens) 
    {
        var lines = ReadFile(inputFilePath);

        var lexer = new Cirno.Lexer.Lexer(lines);
        var tokens = lexer.GetTokens();

        if (isDumpTokens)
        {
            Console.WriteLine("Dump tokens.");
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }
        
        if (lexer.Diagnostics.Count > 0)
        {
            lexer.Diagnostics.Dump(lines);
            System.Environment.Exit(-1);
        }

        var parser = new Cirno.Parser.Parser(tokens, lexer.Diagnostics);
        var exprTree = parser.Parse();

        if (isDumpExpr)
        {
            Console.WriteLine("Dump expression tree.");
            exprTree.Dump();
        }
        
        if (parser.Diagnostics.Count > 0)
        {
            parser.Diagnostics.Dump(lines);
            System.Environment.Exit(-1);
        }

        var ast = new Cirno.AbstractSyntaxTree.AST(exprTree);

        if (isDumpAst)
        {
            Console.WriteLine("Dump ast.");
            ast.Dump();
        }

        var moduleName = System.IO.Path.GetFileNameWithoutExtension(inputFilePath);
        
        using var codeGenVisitor = new Cirno.CodeGen.CodeGenVisitor(moduleName, parser.Diagnostics);
        ast.Root.Accept(codeGenVisitor);

        if (codeGenVisitor.Diagnostics.Count > 0)
        {
            codeGenVisitor.Diagnostics.Dump(lines);
            System.Environment.Exit(-1);
        }

        if (!codeGenVisitor.Verify(out _))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error in code gen.");
            System.Environment.Exit(-1);
        }

        var llvmIrCode = codeGenVisitor.Module.ToString();
        
        if (isEmitIr)
        {
            await System.IO.File.WriteAllTextAsync(outputFilePath, llvmIrCode);
            return;
        }

        await codeGenVisitor.CompileIR2ExeFile(outputFilePath);
    }
}
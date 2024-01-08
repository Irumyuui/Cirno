using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Cirno.AbstractSyntaxTree;
using Cirno.DiagnosticTools;
using Cirno.Lexer;
using Cirno.SyntaxSymbol;
using LLVMSharp.Interop;

namespace Cirno.CodeGen;

public interface ICodeGenVisitable
{
    LLVMValueRef? Accept(ICodeGenVisitor visitor);
}

public interface ICodeGenVisitor
{
    LLVMValueRef? Visit(ArraySubscriptExprNode node);
    LLVMValueRef? Visit(ASTNode node);
    LLVMValueRef? Visit(BinaryOperatorNode node);
    LLVMValueRef? Visit(CallFunctionNode node);
    LLVMValueRef? Visit(CompoundStatementNode node);
    LLVMValueRef? Visit(DeclarationNode node);
    LLVMValueRef? Visit(ExprNode node);
    LLVMValueRef? Visit(FunctionDeclarationNode node);
    LLVMValueRef? Visit(IfStatementNode node);
    LLVMValueRef? Visit<TValue>(IntegerLiteral<TValue> node);
    LLVMValueRef? Visit(LiteralNode node);
    LLVMValueRef? Visit(ProgramNode node);
    LLVMValueRef? Visit(StatementNode node);
    LLVMValueRef? Visit(VariableDeclarationNode node);
    LLVMValueRef? Visit(ReturnStatementNode node);
    LLVMValueRef? Visit(WhileStatementNode node);
}

public sealed class CodeGenVisitor : ICodeGenVisitor, IDisposable
{
    private LLVMContextRef _context;
    private LLVMModuleRef _module;
    private LLVMBuilderRef _irBuilder;
    private bool _isDisposed;
    private EnvSymbolTable _symbolTable;
    private Cirno.DiagnosticTools.DiagnosticList _diagnostics;
    
    public Cirno.DiagnosticTools.DiagnosticList Diagnostics => _diagnostics;
    
    public CodeGenVisitor(string moduleName, DiagnosticTools.DiagnosticList diagnostics)
    {
        _context = LLVMContextRef.Create();
        _module = _context.CreateModuleWithName(moduleName);
        _irBuilder = _context.CreateBuilder();
        
        _symbolTable = new EnvSymbolTable(null);
        _diagnostics = new DiagnosticList(diagnostics);

        InitTarget();
        
        PrevInitBasicEnv();
    }

    private static void InitTarget()
    {
        // wtf
        // where is the loong arch?
        
        // LLVM.InitializeAllTargets();
        LLVM.InitializeAArch64Target();
        LLVM.InitializeAMDGPUTarget();
        LLVM.InitializeARMTarget();
        LLVM.InitializeAVRTarget();
        LLVM.InitializeBPFTarget();
        LLVM.InitializeHexagonTarget();
        LLVM.InitializeLanaiTarget();
        LLVM.InitializeMipsTarget();
        LLVM.InitializeMSP430Target();
        LLVM.InitializeNVPTXTarget();
        LLVM.InitializePowerPCTarget();
        LLVM.InitializeRISCVTarget();
        LLVM.InitializeSparcTarget();
        LLVM.InitializeSystemZTarget();
        LLVM.InitializeVETarget();
        LLVM.InitializeWebAssemblyTarget();
        LLVM.InitializeX86Target();
        LLVM.InitializeXCoreTarget();
        
        // LLVM.InitializeAllTargetMCs();
        LLVM.InitializeAArch64TargetMC();
        LLVM.InitializeAMDGPUTargetMC();
        LLVM.InitializeARMTargetMC();
        LLVM.InitializeAVRTargetMC();
        LLVM.InitializeBPFTargetMC();
        LLVM.InitializeHexagonTargetMC();
        LLVM.InitializeLanaiTargetMC();
        LLVM.InitializeMipsTargetMC();
        LLVM.InitializeMSP430TargetMC();
        LLVM.InitializeNVPTXTargetMC();
        LLVM.InitializePowerPCTargetMC();
        LLVM.InitializeRISCVTargetMC();
        LLVM.InitializeSparcTargetMC();
        LLVM.InitializeSystemZTargetMC();
        LLVM.InitializeVETargetMC();
        LLVM.InitializeWebAssemblyTargetMC();
        LLVM.InitializeX86TargetMC();
        LLVM.InitializeXCoreTargetMC();
        
        // LLVM.InitializeAllDisassemblers();
        LLVM.InitializeAArch64Disassembler();
        LLVM.InitializeAMDGPUDisassembler();
        LLVM.InitializeARMDisassembler();
        LLVM.InitializeAVRDisassembler();
        LLVM.InitializeBPFDisassembler();
        LLVM.InitializeHexagonDisassembler();
        LLVM.InitializeLanaiDisassembler();
        LLVM.InitializeMipsDisassembler();
        LLVM.InitializeMSP430Disassembler();
        LLVM.InitializePowerPCDisassembler();
        LLVM.InitializeRISCVDisassembler();
        LLVM.InitializeSparcDisassembler();
        LLVM.InitializeSystemZDisassembler();
        LLVM.InitializeVEDisassembler();
        LLVM.InitializeWebAssemblyDisassembler();
        LLVM.InitializeX86Disassembler();
        LLVM.InitializeXCoreDisassembler();
        
        // LLVM.InitializeAllAsmParsers();
        LLVM.InitializeAArch64AsmParser();
        LLVM.InitializeAMDGPUAsmParser();
        LLVM.InitializeARMAsmParser();
        LLVM.InitializeAVRAsmParser();
        LLVM.InitializeBPFAsmParser();
        LLVM.InitializeHexagonAsmParser();
        LLVM.InitializeLanaiAsmParser();
        LLVM.InitializeMipsAsmParser();
        LLVM.InitializeMSP430AsmParser();
        LLVM.InitializePowerPCAsmParser();
        LLVM.InitializeRISCVAsmParser();
        LLVM.InitializeSparcAsmParser();
        LLVM.InitializeSystemZAsmParser();
        LLVM.InitializeVEAsmParser();
        LLVM.InitializeWebAssemblyAsmParser();
        LLVM.InitializeX86AsmParser();
        
        // LLVM.InitializeAllAsmPrinters();
        LLVM.InitializeAArch64AsmPrinter();
        LLVM.InitializeAMDGPUAsmPrinter();
        LLVM.InitializeARMAsmPrinter();
        LLVM.InitializeAVRAsmPrinter();
        LLVM.InitializeBPFAsmPrinter();
        LLVM.InitializeHexagonAsmPrinter();
        LLVM.InitializeLanaiAsmPrinter();
        LLVM.InitializeMipsAsmPrinter();
        LLVM.InitializeMSP430AsmPrinter();
        LLVM.InitializeNVPTXAsmPrinter();
        LLVM.InitializePowerPCAsmPrinter();
        LLVM.InitializeRISCVAsmPrinter();
        LLVM.InitializeSparcAsmPrinter();
        LLVM.InitializeSystemZAsmPrinter();
        LLVM.InitializeVEAsmPrinter();
        LLVM.InitializeWebAssemblyAsmPrinter();
        LLVM.InitializeX86AsmPrinter();
        LLVM.InitializeXCoreAsmPrinter();
        
        // LLVM.InitializeAllDisassemblers();
        LLVM.InitializeAArch64Disassembler();
        LLVM.InitializeAMDGPUDisassembler();
        LLVM.InitializeARMDisassembler();
        LLVM.InitializeAVRDisassembler();
        LLVM.InitializeBPFDisassembler();
        LLVM.InitializeHexagonDisassembler();
        LLVM.InitializeLanaiDisassembler();
        LLVM.InitializeMipsDisassembler();
        LLVM.InitializeMSP430Disassembler();
        LLVM.InitializePowerPCDisassembler();
        LLVM.InitializeRISCVDisassembler();
        LLVM.InitializeSparcDisassembler();
        LLVM.InitializeSystemZDisassembler();
        LLVM.InitializeVEDisassembler();
        LLVM.InitializeWebAssemblyDisassembler();
        LLVM.InitializeX86Disassembler();
        LLVM.InitializeXCoreDisassembler();
    }
    
    private void PrevInitBasicEnv()
    {
        // c scanf
        var scanfFnRetTy = _context.Int32Type;
        LLVMTypeRef[] scanfFnParamsTy = [LLVMTypeRef.CreatePointer(_context.Int8Type, 0)];
        var scanfFnTy = LLVMTypeRef.CreateFunction(scanfFnRetTy, scanfFnParamsTy, true);
        var scanfFn = _module.AddFunction("scanf", scanfFnTy);

        _symbolTable.Add("scanf",
            new FunctionSymbol(new SyntaxToken(SyntaxKind.FunctionExpression, "scanf", 0, 0), "scanf", scanfFn,
                scanfFnTy));
        
        // c printf
        var printfFnRetTy = _context.Int32Type;
        LLVMTypeRef[] printfFnParamsTy = [LLVMTypeRef.CreatePointer(_context.Int8Type, 0)];
        var printfFnTy = LLVMTypeRef.CreateFunction(printfFnRetTy, printfFnParamsTy, true);
        var printfFn = _module.AddFunction("printf", printfFnTy);

        _symbolTable.Add("printf",
            new FunctionSymbol(new SyntaxToken(SyntaxKind.FunctionExpression, "printf", 0, 0), "printf", scanfFn,
                scanfFnTy));

        // fn input() -> i32
        var inputFnRetTy = _context.Int32Type;
        LLVMTypeRef[] inputFnParamsTy = [];
        var inputFnTy = LLVMTypeRef.CreateFunction(inputFnRetTy, inputFnParamsTy);
        var inputFn = _module.AddFunction("input", inputFnTy);

        var inputEntry = inputFn.AppendBasicBlock("entry");
        _irBuilder.PositionAtEnd(inputEntry);
        var inputValue = _irBuilder.BuildAlloca(_context.Int32Type);
        var inputFormat = _irBuilder.BuildGlobalStringPtr("%d");
        _irBuilder.BuildCall2(scanfFnTy, scanfFn, [inputFormat, inputValue]);
        _irBuilder.BuildRet(_irBuilder.BuildLoad2(_context.Int32Type, inputValue));
        
        _symbolTable.Add("input",
            new FunctionSymbol(new SyntaxToken(SyntaxKind.FunctionExpression, "input", 0, 0), "input", inputFn, inputFnTy));
        
        // fn output(x: i32)
        var outputFnRetTy = _context.VoidType;
        LLVMTypeRef[] outputFnParamsTy = [_context.Int32Type];
        var outputFnTy = LLVMTypeRef.CreateFunction(outputFnRetTy, outputFnParamsTy);
        var outputFn = _module.AddFunction("output", outputFnTy);

        var outputEntry = outputFn.AppendBasicBlock("entry");
        _irBuilder.PositionAtEnd(outputEntry);
        _irBuilder.BuildCall2(printfFnTy, printfFn, [_irBuilder.BuildGlobalStringPtr("%d\n"), outputFn.Params[0]]);
        _irBuilder.BuildRetVoid();
        
        _symbolTable.Add("output",
            new FunctionSymbol(new SyntaxToken(SyntaxKind.FunctionExpression, "output", 0, 0), "output", outputFn, outputFnTy));
        
        _irBuilder.ClearInsertionPosition();
    }
    
    ~CodeGenVisitor()
    {
        Dispose(false);
    }
    
    public void Dispose()
    {
        Dispose(true);
        System.GC.SuppressFinalize(this);
    }
    
    private void Dispose(bool disposing)
    {
        if (_isDisposed) 
            return;
        
        if (disposing)
        {
            
        }
        
        _irBuilder.Dispose();
        _module.Dispose();
        _context.Dispose();

        _isDisposed = true;
    }

    public void Dump()
    {
        _module.Dump();
    }
    
    public LLVMValueRef? Visit(ArraySubscriptExprNode node)
    {
        if (!_symbolTable.TryGetSymbolFromLinkTable(node.Name, out var arrayValue))
        {
            _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col), "");
            return null;
        }

        if (arrayValue is null)
        {
            _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col), "");
            return null;
        }
        
        if (arrayValue?.Value.TypeOf.Kind is not LLVMTypeKind.LLVMPointerTypeKind)
        {
            _diagnostics.ReportNotExpectType(new TextLocation(node.Position.Line, node.Position.Col),
                arrayValue?.Name ?? "", ValueTypeKind.IntArray, arrayValue?.TypeKind ?? ValueTypeKind.Void);
            return null;
        }

        var maybeIndex = node.OffsetExpr.Accept(this);
        if (maybeIndex is null)
        {
            _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col),
                $"Invalid offset for {arrayValue?.Name ?? ""}");
            return null;
        }

        var arr = arrayValue!.Value;
        
        var index = maybeIndex.Value;
        if (index.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind)
        {
            var idx = _irBuilder.BuildLoad2(_context.Int32Type, index);
            var item = _irBuilder.BuildGEP2(_context.Int32Type, arr, [idx]);
            return item;
        }
        else if (index.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind)
        {
            var item = _irBuilder.BuildInBoundsGEP2(_context.Int32Type, arr, [index]);
            return item;
        }
        
        _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col), 
            $"Array index must be an integer.");
        return null;
    }

    public LLVMValueRef? Visit(ASTNode node)
    {
        // throw new NotImplementedException();
        foreach (var nextNode in node.GetChildren())
        {
            nextNode.Accept(this);
        }

        return null;
    }

    public LLVMValueRef? Visit(BinaryOperatorNode node)
    {
        System.Diagnostics.Debug.WriteLine("In BinaryOperatorNode");
        System.Diagnostics.Debug.WriteLine(_module);

        var leftResult = node.Left.Accept(this);
        var rightResult = node.Right!.Accept(this);

        if (leftResult is null || rightResult is null)
        {
            return null;
        }

        var left = leftResult.Value;
        var right = rightResult.Value;

        return node.OpKind switch
        {
            BinaryOperatorKind.Addition => BuildAdd(left, right, node),
            BinaryOperatorKind.Subtraction => BuildSub(left, right, node),
            BinaryOperatorKind.Multiplication => BuildMul(left, right, node),
            BinaryOperatorKind.Division => BuildDiv(left, right, node),
            BinaryOperatorKind.Assignment => BuildAssign(left, right, node),
            BinaryOperatorKind.EqualTo => BuildCmp(left, right, LLVMIntPredicate.LLVMIntEQ, node),
            BinaryOperatorKind.NotEqualTo => BuildCmp(left, right, LLVMIntPredicate.LLVMIntNE, node),
            BinaryOperatorKind.LessThan => BuildCmp(left, right, LLVMIntPredicate.LLVMIntSLT, node),
            BinaryOperatorKind.LessThanOrEqualTo => BuildCmp(left, right, LLVMIntPredicate.LLVMIntSLE, node),
            BinaryOperatorKind.GreaterThanOrEqualTo => BuildCmp(left, right, LLVMIntPredicate.LLVMIntSGE, node),
            BinaryOperatorKind.GreaterThan => BuildCmp(left, right, LLVMIntPredicate.LLVMIntSGT, node),
            BinaryOperatorKind.Unknown => null,
            null => null,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private LLVMValueRef? BuildAssign(LLVMValueRef left, LLVMValueRef right, BinaryOperatorNode node)
    {
        if (left.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind)
        {
            if (right.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind)
            {
                _irBuilder.BuildStore(right, left);
                return left;
            }

            if (right.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind)
            {
                _irBuilder.BuildStore(_irBuilder.BuildLoad2(_context.Int32Type, right), left);
                return left;
            }

            _diagnostics.ReportUnknownValueTypeError(new TextLocation(node.Position.Line, node.Position.Line));
            return null;
        }
        else
        {
            _diagnostics.ReportNotLeftValueError(new TextLocation(node.Position.Line, node.Position.Col), node.Left.Name);
            return null;
        } 
    }
    
    private LLVMValueRef? BuildAdd(LLVMValueRef left, LLVMValueRef right, BinaryOperatorNode node)
    {
        if (left.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind &&
            right.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind) {
            return _irBuilder.BuildNSWAdd(
                _irBuilder.BuildLoad2(_context.Int32Type, left),
                _irBuilder.BuildLoad2(_context.Int32Type, right)
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind) {
            return _irBuilder.BuildNSWAdd(
                _irBuilder.BuildLoad2(_context.Int32Type, left),
                right
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind) {
            return _irBuilder.BuildNSWAdd(
                left,
                _irBuilder.BuildLoad2(_context.Int32Type, right)
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind) {
            return _irBuilder.BuildNSWAdd(left, right);
        }

        _diagnostics.ReportUnknownValueTypeError(new TextLocation(node.Position.Line, node.Position.Line));
        return null;
    }

    private LLVMValueRef? BuildSub(LLVMValueRef left, LLVMValueRef right, BinaryOperatorNode node)
    {
        if (left.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind &&
            right.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind) {
            return _irBuilder.BuildNSWSub(
                _irBuilder.BuildLoad2(_context.Int32Type, left),
                _irBuilder.BuildLoad2(_context.Int32Type, right)
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind) {
            return _irBuilder.BuildNSWSub(
                _irBuilder.BuildLoad2(_context.Int32Type, left),
                right
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind) {
            return _irBuilder.BuildNSWSub(
                left,
                _irBuilder.BuildLoad2(_context.Int32Type, right)
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind) {
            return _irBuilder.BuildNSWSub(left, right);
        }

        _diagnostics.ReportUnknownValueTypeError(new TextLocation(node.Position.Line, node.Position.Line));
        return null;
    }
    
    private LLVMValueRef? BuildMul(LLVMValueRef left, LLVMValueRef right, BinaryOperatorNode node)
    {
        if (left.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind &&
            right.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind) {
            return _irBuilder.BuildNSWMul(
                _irBuilder.BuildLoad2(_context.Int32Type, left),
                _irBuilder.BuildLoad2(_context.Int32Type, right)
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind) {
            return _irBuilder.BuildNSWMul(
                _irBuilder.BuildLoad2(_context.Int32Type, left),
                right
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind) {
            return _irBuilder.BuildNSWMul(
                left,
                _irBuilder.BuildLoad2(_context.Int32Type, right)
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind) {
            return _irBuilder.BuildNSWMul(left, right);
        }

        _diagnostics.ReportUnknownValueTypeError(new TextLocation(node.Position.Line, node.Position.Line));
        return null;
    }
    
    private LLVMValueRef? BuildDiv(LLVMValueRef left, LLVMValueRef right, BinaryOperatorNode node)
    {
        if (left.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind &&
            right.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind) {
            return _irBuilder.BuildSDiv(
                _irBuilder.BuildLoad2(_context.Int32Type, left),
                _irBuilder.BuildLoad2(_context.Int32Type, right)
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind) {
            return _irBuilder.BuildSDiv(
                _irBuilder.BuildLoad2(_context.Int32Type, left),
                right
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind) {
            return _irBuilder.BuildSDiv(
                left,
                _irBuilder.BuildLoad2(_context.Int32Type, right)
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind) {
            return _irBuilder.BuildSDiv(left, right);
        }

        _diagnostics.ReportUnknownValueTypeError(new TextLocation(node.Position.Line, node.Position.Line));
        return null;
    }

    private LLVMValueRef? BuildCmp(LLVMValueRef left, LLVMValueRef right, LLVMIntPredicate opt, BinaryOperatorNode node)
    {
        if (left.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind &&
            right.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind) {
            return _irBuilder.BuildICmp(
                opt,
                _irBuilder.BuildLoad2(_context.Int32Type, left),
                _irBuilder.BuildLoad2(_context.Int32Type, right)
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind) {
            return _irBuilder.BuildICmp(
                opt,
                _irBuilder.BuildLoad2(_context.Int32Type, left),
                right
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind) {
            return _irBuilder.BuildICmp(
                opt,
                left,
                _irBuilder.BuildLoad2(_context.Int32Type, right)
            );
        } else if (left.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind &&
                   right.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind) {
            return _irBuilder.BuildICmp(opt, left, right);
        }

        _diagnostics.ReportUnknownValueTypeError(new TextLocation(node.Position.Line, node.Position.Line));
        return null;
    }

    public LLVMValueRef? Visit(CallFunctionNode node)
    {
        if (!_symbolTable.TryGetSymbolFromLinkTable(node.Name, out var symbol) && symbol is null)
        {
            _diagnostics.ReportUndefinedVariableError(new TextLocation(node.Position.Line, node.Position.Col),
                node.Name);
            return null;
        }

        var func = symbol!.Value;
        if (symbol.TypeKind is not ValueTypeKind.Function)
        {
            _diagnostics.ReportNotExpectType(new TextLocation(node.Position.Line, node.Position.Col), node.Name,
                ValueTypeKind.Function, symbol.TypeKind);
            return null;
        }

        if (node.Args.Length != func.Params.Length)
        {
            _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col),
                $"Insufficient parameters for function {node.Name} call.");
            return null;
        }

        Span<LLVMValueRef> args = stackalloc LLVMValueRef[func.Params.Length];
        var error = false;
        for (var i = 0; i < args.Length; i++)
        {
            var result = node.Args[i].Accept(this);
            if (result is null || error)
            {
                error = true;
                continue;
            }

            args[i] = result.Value.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind
                ? _irBuilder.BuildLoad2(func.Params[i].TypeOf, result.Value)
                : result.Value;

            if (args[i].TypeOf.Kind != func.Params[i].TypeOf.Kind)
            {
                var expectTypeKind = func.Params[i].TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind
                    ? ValueTypeKind.Int
                    : ValueTypeKind.IntArray;

                _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Line),
                    $"Wrong parameter {i} when calling function {node.Name}, expected to be {expectTypeKind}");
            }
        }

        return error ? null : _irBuilder.BuildCall2(symbol.Type, func, args.ToArray());
    }

    public LLVMValueRef? Visit(CompoundStatementNode node)
    {
        _symbolTable = new EnvSymbolTable(_symbolTable);

        foreach (var declarationNode in node.DeclarationStatement)
        {
            declarationNode.Accept(this);
        }

        foreach (var stmtNode in node.ExpressionStatement)
        {
            var result = stmtNode.Accept(this);

            if (stmtNode is not ReturnStatementNode)
                continue;
            
            _symbolTable = _symbolTable.PrevTable ?? new EnvSymbolTable(null);
            return result;
        }
        
        _symbolTable = _symbolTable.PrevTable ?? new EnvSymbolTable(null);
        return null;
    }

    public LLVMValueRef? Visit(DeclarationNode node)
    {
        throw new NotImplementedException();
    }

    public LLVMValueRef? Visit(ExprNode node)
    {
        throw new NotImplementedException();
    }

    public LLVMValueRef? Visit(FunctionDeclarationNode node)
    {
        // defined a function
        System.Diagnostics.Debug.WriteLine(_module);

        if (node.Parameters.Select(item => item.Name).Distinct().Count() < node.Parameters.Length)
        {
            _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col),
                $"Function {node.Name} parameter redefinition.");
            return null;
        }
        
        LLVMTypeRef funcRetTy;
        if (node.FunctionType.FirstOrDefault() is LiteralType.Int)
        {
            funcRetTy = _context.Int32Type;
        } else if (node.FunctionType.FirstOrDefault() is LiteralType.Void)
        {
            funcRetTy = _context.VoidType;
        } else
        {
            _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col),
                "Function return value can only be int or void.");
            return null;
        }

        var funcParamsTy = new LLVMTypeRef[node.Parameters.Length];
        for (int i = 0; i < node.Parameters.Length; i++)
        {
            if (node.Parameters[i].Type is LiteralType.Int)
            {
                funcParamsTy[i] = _context.Int32Type;
            } else if (node.Parameters[i].Type is LiteralType.IntPtr)
            {
                funcParamsTy[i] = LLVMTypeRef.CreatePointer(_context.Int32Type, 0);
            }
            else
            {
                _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col),
                    "Function declaration parameter type must be int or intPtr.");
                return null;
            }
        }
        
        var funcTy = LLVMTypeRef.CreateFunction(funcRetTy, funcParamsTy);

        if (_symbolTable.ContainsKey(node.Name))
        {
            _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col), $"Redefine variable {node.Name}.");
            return null;
        }
        
        var func = _module.AddFunction(node.Name, funcTy);
        _symbolTable.Add(node.Name,
            new FunctionSymbol(
                new SyntaxToken(SyntaxKind.FunctionExpression, node.Name, node.Position.Line, node.Position.Col),
                node.Name, func, funcTy));

        _symbolTable = new EnvSymbolTable(_symbolTable);

        // function params alloc
        
        var entry = func.AppendBasicBlock("entry");
        _irBuilder.PositionAtEnd(entry);

        if (node.Body.DeclarationStatement.Length is 0 && node.Body.ExpressionStatement.Length is 0)
        {
            if (funcRetTy.Kind is LLVMTypeKind.LLVMVoidTypeKind) 
                return _irBuilder.BuildRetVoid();
            
            _diagnostics.ReportSemanticWarning(new TextLocation(node.Position.Line, node.Position.Col),
                $"Function {node.Name} has no return value.");
            return _irBuilder.BuildRet(LLVMValueRef.CreateConstInt(_context.Int32Type, 1, true));
        }

        for (int i = 0; i < func.Params.Length; i++)
        {
            var (item, name) = (func.Params[i], node.Parameters[i].Name);

            var value = _irBuilder.BuildAlloca(item.TypeOf, name);
            _irBuilder.BuildStore(item, value);
            _symbolTable.Add(name,
                new ValueSymbol(new SyntaxToken(SyntaxKind.Identifier, name, node.Position.Line, node.Position.Col),
                    name, value, value.TypeOf,
                    item.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind ? ValueTypeKind.IntArray : ValueTypeKind.Int, ValueScopeKind.Local));
        }
        
        // function body construct
        
        var compBasicBlock = func.AppendBasicBlock("comps");
        _irBuilder.BuildBr(compBasicBlock);
        _irBuilder.PositionAtEnd(compBasicBlock);

        var hasBreakRet = false;
        foreach (var next in node.Body.DeclarationStatement)
        {
            next.Accept(this);
        }

        foreach (var next in node.Body.ExpressionStatement)
        {
            var result = next.Accept(this);
            if (result is not null)
            {
                hasBreakRet = next switch
                {
                    IfStatementNode => true,
                    ReturnStatementNode => true,
                    WhileStatementNode => true,
                    _ => false
                };
            }

            if (hasBreakRet)
            {
                break;
            }
        }

        //_ = node.Body.Accept(this, compBasicBlock, exitBasicBlock);

        //System.Diagnostics.Debug.WriteLine(_module);

        if (!hasBreakRet) {
            if (funcRetTy.Kind is LLVMTypeKind.LLVMVoidTypeKind)
                return _irBuilder.BuildRetVoid();

            _diagnostics.ReportSemanticWarning(new TextLocation(node.Position.Line, node.Position.Col),
                $"Function {node.Name} has no return value.");
            return _irBuilder.BuildRet(LLVMValueRef.CreateConstInt(_context.Int32Type, 1, true));
        }

        _symbolTable = _symbolTable.PrevTable!;
        
        return null;
    }

    public LLVMValueRef? Visit(IfStatementNode node)
    {
        if (node.Body.Length is 0)
        {
            return null;
        }
        
        var result = node.Expr.Accept(this);
        if (result is null)
        {
            return null;
        }

        LLVMValueRef ret;
        if (result.Value.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind &&
            result.Value.TypeOf.ElementType.Kind is LLVMTypeKind.LLVMIntegerTypeKind)
        {
            ret = _irBuilder.BuildLoad2(result.Value.TypeOf.ElementType, result.Value);
        } else if (result.Value.TypeOf.Kind is LLVMTypeKind.LLVMIntegerTypeKind)
        {
            ret = result.Value;
        }
        else
        {
            return null;
        }

        var func = _irBuilder.InsertBlock.Parent;

        if (node.Body.Length is 1)
        {
            var yesDoBasicBlock = func.AppendBasicBlock("if_then");
            var noDoBasicBlock = func.AppendBasicBlock("if_not");

            _irBuilder.BuildCondBr(_irBuilder.BuildICmp(LLVMIntPredicate.LLVMIntNE, ret,
                LLVMValueRef.CreateConstInt(ret.TypeOf, 1)), yesDoBasicBlock, noDoBasicBlock);
            
            _irBuilder.PositionAtEnd(yesDoBasicBlock);
            if (node.Body[0].Accept(this) is null)
            {
                _irBuilder.BuildBr(noDoBasicBlock);
            }

            _irBuilder.PositionAtEnd(noDoBasicBlock);
            
            return null;
        }
        
        var thenEntryBlock = func.AppendBasicBlock("if_then");
        var elseEntryBlock = func.AppendBasicBlock("if_else");
        // var mergeEntryBlock = func.AppendBasicBlock("if_merge");

        _irBuilder.BuildCondBr(_irBuilder.BuildICmp(LLVMIntPredicate.LLVMIntNE, ret,
            LLVMValueRef.CreateConstInt(ret.TypeOf, 1)), thenEntryBlock, elseEntryBlock);
        
        // then
        System.Diagnostics.Debug.WriteLine("In if stmt => then");
        System.Diagnostics.Debug.WriteLine(_module);
        _irBuilder.PositionAtEnd(thenEntryBlock);
        var result1 = node.Body[0].Accept(this);
        
        // if (node.Body[0].Accept(this, entryBasicBlock, exitBasicBlock) is null)
        // {
        //     // node.Body[0].Accept(this, entryBasicBlock, exitBasicBlock);
        //     _irBuilder.BuildBr(mergeEntryBlock);
        // }

        System.Diagnostics.Debug.WriteLine("In if stmt => else");
        System.Diagnostics.Debug.WriteLine(_module);
        
        // else
        _irBuilder.PositionAtEnd(elseEntryBlock);
        var result2 = node.Body[1].Accept(this);
        
        // if (node.Body[1].Accept(this, entryBasicBlock, exitBasicBlock) is null)
        // {
        //     // node.Body[1].Accept(this, entryBasicBlock, exitBasicBlock);
        //     _irBuilder.BuildBr(mergeEntryBlock);
        // }

        if (result1 is not null && result2 is not null)
        {
            return result2;
        }
        
        var mergeEntryBlock = func.AppendBasicBlock("if_merge");
        _irBuilder.PositionAtEnd(thenEntryBlock);
        
        _irBuilder.BuildBr(mergeEntryBlock);
        
        _irBuilder.PositionAtEnd(elseEntryBlock);
        _irBuilder.BuildBr(mergeEntryBlock);
        
        _irBuilder.PositionAtEnd(mergeEntryBlock);
        
        System.Diagnostics.Debug.WriteLine("In if stmt => merge");
        System.Diagnostics.Debug.WriteLine(_module);
        
        return null;
    }

    public LLVMValueRef? Visit<TValue>(IntegerLiteral<TValue> node)
    {
        if (node.Value is int v1)
        {
            return LLVMValueRef.CreateConstInt(_context.Int32Type, Convert.ToUInt64(long.Abs(v1)), v1 < 0);
        }

        _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col),
            $"Error Integer Type {node.Value?.GetType().Name}");
        return null;
    }

    public LLVMValueRef? Visit(LiteralNode node)
    {
        if (!_symbolTable.TryGetSymbolFromLinkTable(node.Name, out var result) || result is null)
        {
            _diagnostics.ReportUndefinedVariableError(new TextLocation(node.Position.Line, node.Position.Col),
                node.Name);
            return null;
        }

        if (result.Value.TypeOf.Kind is not LLVMTypeKind.LLVMPointerTypeKind) {
            _diagnostics.ReportNotLeftValueError(new TextLocation(node.Position.Line, node.Position.Col), node.Name);
            return null;
        }

        return result.Value;
    }

    public LLVMValueRef? Visit(ProgramNode node)
    {
        if (node.DeclarationNodes.LastOrDefault(item => item.NodeType is ASTNodeType.FunctionDeclaration)
                ?.Name is not "main")
        {
            _diagnostics.ReportSemanticError(TextLocation.NoPosition, "Last function must be main.");
        }

        foreach (var exprNode in node.DeclarationNodes)
        {
            exprNode.Accept(this);
        }

        return null;
    }

    public LLVMValueRef? Visit(StatementNode node)
    {
        throw new NotImplementedException();
    }

    public LLVMValueRef? Visit(VariableDeclarationNode node)
    {
        // throw new NotImplementedException();
        if (node.Type is not LiteralType.Int and not LiteralType.IntPtr)
        {
            _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col),
                $"Variable definition must be of type int or int[].");
            return null;
        }

        if (_symbolTable.CurrentContains(node.Name))
        {
            _diagnostics.ReportSemanticError(new TextLocation(node.Position.Line, node.Position.Col),
                $"Variable {node.Name} redefined.");
        }

        LLVMTypeRef valueTy;
        if (node.Type is LiteralType.Int)
        {
            valueTy = _context.Int32Type;
        }
        else
        {
            int len = node.ArrayLength!.Value;
            valueTy = LLVMTypeRef.CreateArray(_context.Int32Type, (uint)len);
        }

        LLVMValueRef value;
        if (_symbolTable.PrevTable is null)
        {
            value = _module.AddGlobal(valueTy, node.Name);
        }
        else
        {
            value = _irBuilder.BuildAlloca(valueTy, node.Name);
        }

        System.Diagnostics.Debug.WriteLine(value.TypeOf);

        _symbolTable.Add(node.Name,
            new ValueSymbol(new SyntaxToken(SyntaxKind.Identifier, node.Name, node.Position.Line, node.Position.Col),
                node.Name, value, valueTy, node.Type is LiteralType.Int ? ValueTypeKind.Int : ValueTypeKind.IntArray, _symbolTable.PrevTable is null ? ValueScopeKind.Global : ValueScopeKind.Local));

        System.Diagnostics.Debug.WriteLine("In VariableDeclarationNode");
        System.Diagnostics.Debug.WriteLine(_module);

        return null;
    }

    public LLVMValueRef? Visit(ReturnStatementNode node)
    {
        //if (entryBasicBlock is null)
        //{
        //    throw new ArgumentNullException(nameof(entryBasicBlock));
        //}
        
        //_irBuilder.PositionAtEnd(entryBasicBlock.Value);
        
        var result = node.ReturnExpr?.Accept(this);
        if (result is null)
        {
            return _irBuilder.BuildRetVoid();
        }

        if (result.Value.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind)
        {
            var basicType = _context.Int32Type;
            return _irBuilder.BuildRet(
                    _irBuilder.BuildLoad2(basicType, result.Value)
                );
        }
        else
        {
            return _irBuilder.BuildRet(result.Value);
        }
    }

    public LLVMValueRef? Visit(WhileStatementNode node)
    {
        System.Diagnostics.Debug.WriteLine(_module);

        // throw new NotImplementedException();
        
        var func = _irBuilder.InsertBlock.Parent;
        
        var logicBasicBlock = func.AppendBasicBlock("logic_comp");
        var loopBasicBlock = func.AppendBasicBlock("loop");
        var loopEndBasicBlock = func.AppendBasicBlock("loop_end");

        _irBuilder.BuildBr(logicBasicBlock);

        _irBuilder.PositionAtEnd(logicBasicBlock);
        var comp = node.Expr.Accept(this);
        if (comp is null)
        {
            return null;
        }

        var compValue = comp.Value.TypeOf.Kind is LLVMTypeKind.LLVMPointerTypeKind
            ? _irBuilder.BuildLoad2(comp.Value.TypeOf.ElementType, comp.Value)
            : comp.Value;
     
        _irBuilder.BuildCondBr(
            _irBuilder.BuildICmp(LLVMIntPredicate.LLVMIntNE, compValue,
                LLVMValueRef.CreateConstInt(compValue.TypeOf, 1)), loopBasicBlock, loopEndBasicBlock);

        _irBuilder.PositionAtEnd(loopBasicBlock);
        node.CompoundStatement.Accept(this);
        _irBuilder.BuildBr(logicBasicBlock);
        
        _irBuilder.PositionAtEnd(loopEndBasicBlock);

        return null;
    }

    public void Verify()
    {
        _module.Verify(LLVMVerifierFailureAction.LLVMAbortProcessAction);
    }
}
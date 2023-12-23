namespace Cirno.Lexer;

public enum SyntaxKind {    
    Void,
    Int,
    If,
    Else,
    While,
    Return,

    Plus,
    Minus,
    Asterisk,
    Slash,

    Assign,

    LessThan,
    LessThanOrEqualTo,
    EqualTo,
    GreaterThanOrEqualTo,
    GreaterThan,
    NotEqualTo,

    Comma,
    Semicolon,

    OpenRoundBrackets,
    CloseRoundBrackets,

    OpenSquareBrackets,
    CloseSquareBrackets,

    OpenCurlyBrackets,
    CloseCurlyBrackets,

    // Mark
    Identifier,
    IdentifierOrKeyword,
    Number,
    
    EndOfFile,
    Unknown,
    CommentsEnd,

    // help
    Type,

    NumberOrIdentifier,
    IntPtrRef,

    NumberExpression,
    AdditionExpression,
    SubtractionExpression,
    MultiplicationExpression,
    DivisionExpression,
    LessThanExpression,
    LessThanOrEqualToExpression,
    EqualToExpression,
    GreaterThanOrEqualToExpression,
    GreaterThanExpression,
    NotEqualToExpression,
    DeclarationListExpression,
    DeclarationExpression,
    ParamExpression,
    ParamListExpression,
    FunctionExpression,
    VarDeclarationExpression,
    CompoundStatementExpression,
    LocalDeclarationsExpression,
    LocalDeclarationTailExpression,
    DeclarationListTailExpression,
    ParamListTailExpression,
    ReturnExpression,
    SelectionStatementExpression,
    StatementListExpression,
    StatementListTailExpression,
    IterationStatementExpression,
    AssignmentExpression,
    ExpressionStatementExpression,
    VariableExpression,
    IdentifierOrNumber,
    SimpleExpression,
    AdditiveExpression,
    AdditiveTailExpression,
    TermExpression,
    TermTailExpression,
    FactorExpression,
    CallFunctionExpression,
    CallFuncArgsExpression,
    ArgsExpression,
    ArgsListExpression,
    ArgsListTailExpression,
}

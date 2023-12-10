namespace Cirno;

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
    AdditonExpresion,
    SubtractionExpression,
    MultiplicationExpression,
    DivisionExpression,
    LessThanExpression,
    LessThanOrEqualToExpression,
    EqualToExpression,
    GreaterThanOrEqualToExpression,
    GreaterThanExpression,
    NotEqualToExression,
    DeclarationListExpression,
    DeclarationExpression,
    ParamExpression,
    ParamListExpression,
    FunctionExpression,
    VarDeclarationExpression,
    CompoundStatementExpression,
    LocalDecarationsExpression,
    LocalDecarationTailExpression,
    DeclarationListTailExpression,
    ParamListTailExpression,
    ReturnExpression,
    SelectionStatementExpression,
    StatementListExpression,
    StatementListTailExpression,
    IterationStatementEpxression,
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

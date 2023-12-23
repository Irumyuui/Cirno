using System;
using System.Collections.Generic;

namespace Cirno.Lexer;

public static class Automata
{
    private struct AutomataNode(SyntaxKind kind, List<Func<char, int?>> next)
    {
        public SyntaxKind Kind { get; set; } = kind;

        public List<Func<char, int?>> Next { get; set; } = next;
    }

    private static List<AutomataNode> AdjustList { get; set; }

    // TODO: Waiting for better implementation
    static Automata()
    {
        AdjustList = [
            // 0
            new(SyntaxKind.Unknown, [
                next => char.IsLetter(next) ? 1 : null,
                next => char.IsDigit(next) ? 2 : null,
                next => next is '<' ? 3 : null,
                next => next is '>' ? 4 : null,
                next => next is '=' ? 5 : null,
                next => next is '!' ? 6 : null,
                next => next is '/' ? 7 : null,
                next => next is '+' ? 8 : null,
                next => next is '-' ? 9 : null,
                next => next is '*' ? 10 : null,

                next => next is ',' ? 18 : null,
                next => next is ';' ? 19 : null,
                next => next is '(' ? 20 : null,
                next => next is ')' ? 21 : null,
                next => next is '{' ? 22 : null,
                next => next is '}' ? 23 : null,
                next => next is '[' ? 24 : null,
                next => next is ']' ? 25 : null,
            ]),

            // 1
            new(SyntaxKind.IdentifierOrKeyword, [
                next => char.IsLetter(next) ? 1 : null
            ]),

            // 2
            new(SyntaxKind.Number, [
                next => char.IsNumber(next) ? 2 : null,
                // next => next is '.' ? 26 : null
            ]),

            // 3 
            new(SyntaxKind.LessThan, [
                next => next is '=' ? 11 : null
            ]),

            // 4
            new(SyntaxKind.GreaterThan, [
                next => next is '=' ? 12 : null
            ]),

            // 5
            new(SyntaxKind.Assign, [
                next => next is '=' ? 13 : null
            ]),

            // 6
            new(SyntaxKind.Unknown, [
                next => next is '=' ? 14 : null
            ]),

            // 7
            new(SyntaxKind.Slash, [
                next => next is '*' ? 15 : null
            ]),

            // 8
            new(SyntaxKind.Plus, []),
            // 9
            new(SyntaxKind.Minus, []),
            // 10
            new(SyntaxKind.Asterisk, []),

            // 11
            new(SyntaxKind.LessThanOrEqualTo, []),
            // 12
            new(SyntaxKind.GreaterThanOrEqualTo, []),
            // 13
            new(SyntaxKind.EqualTo, []),
            // 14
            new(SyntaxKind.NotEqualTo, []),

            // 15
            new(SyntaxKind.Unknown, [
                next => next is '*' ? 16 : 15
            ]),
            // 16
            new(SyntaxKind.Unknown, [
                next => next switch {
                    '/' => 17,
                    _ => 15
                }
            ]),
            
            // 17
            new(SyntaxKind.CommentsEnd, []),

            // 18
            new(SyntaxKind.Comma, []),
            // 19
            new(SyntaxKind.Semicolon, []),
            // 20
            new(SyntaxKind.OpenRoundBrackets, []),
            // 21
            new(SyntaxKind.CloseRoundBrackets, []),
            // 22
            new(SyntaxKind.OpenCurlyBrackets, []),
            // 23
            new(SyntaxKind.CloseCurlyBrackets, []),
            // 24
            new(SyntaxKind.OpenSquareBrackets, []),
            // 25
            new(SyntaxKind.CloseSquareBrackets, []),

            // 26
            new(SyntaxKind.Unknown, [
                next => char.IsDigit(next) ? 27 : null
            ]),
            // // 27
            // new(SyntaxTokenKind.ConstantFloat, [
            //     next => char.IsDigit(next) ? 27 : null
            // ])
        ];
    }

    public static int? NextState(int id, char next)
    {
        foreach (var func in AdjustList[id].Next) {
            var result = func(next);
            if (result is not null) {
                return result;
            }
        }
        return null;
    }

    public static SyntaxKind GetStateKind(int id) => AdjustList[id].Kind;
}

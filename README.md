# Cirno

## 未来实现目标

- 重构 `Parser` 部分：一步生成 `ast`
- 重构 `LLVM IR` 生成部分：重新定义类型，不再以 `LLVMSharp` 提供的类型作为支持
- ...

## 简介

这是一个使用 `c#` 实现的关于  `c minus` （简称 `c-`）的简单编译器，它将 `c-` 代码翻译为 `llvm ir` ，最后由 `LLVM` 将 `LLVM IR` 翻译为目标代码。

## 环境

该项目目前只支持 `linux` ，不支持 `windows` 。

确保 `dotnet` 版本至少为 `8.0.100` 。

## 关于 C Minus

`c minus` 是 `c` 的一个子集语言。

有关 `c minus` ，可以查看文件 [c-minus.pdf](c-minus.pdf)

## 整体流程

```text
源代码 => 词法分析器 => 语法分析器 => 中间代码生成 => 目标代码生成
```

## 整体结构

### 词法分析器

词法分析器 `Lexer` 实现比较简单，使用一个 `DFA` 进行状态转移。

### 语法分析器

语法分析器使用递归下降语法分析构造出具体的语法树，之后从构造出的分析树构造出精简的语法分析树。

### 中间代码生成

中间代码生成与语义分析一起，这部分将生成 `LLVM IR`。

### 目标代码生成

生成的 `LLVM IR` 将通过 `LLVM` 生成目标代码。

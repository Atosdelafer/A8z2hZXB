---
author: Winand, Roald, Sjoerd, Alexey and Anvar
title: Concepts of programming languages
subtitle: Prolog
theme: uucs
mainfont: Ubuntu Light
sansfont: Ubuntu Light
---
# What is Prolog?

* A general-purpose logic programming language.
* One of the first and most popular logic programming language available.
* Originally intended as a way to process natural language.

SWI Prolog (http://www.swi-prolog.org/) one of the most mature implementations of Prolog.

---
# What is logic programming?
In logic programming there are we have two main elements, a database and queries on this database. The database should contain all the information that we value, or at least imply this information.
The queries are used to extract this valuable information from this database.
---
# What does this database consist of?
The database consists of a collection of logical rules, which are called clauses.
We can start by the most simple clause: a fact:
CheeseisLife.
Now if we would query "CheeseisLife." we would get True.
PizzaisLife :- CheeseisLife.
would make the query PizzaisLife become True as CheeseisLife is True.
---
# More interesting clauses
Say we want to have a database with guests of a party, we can denote in the database our guests and their resp. gender in the following way.
woman(jody).
woman(alice). # alice? who the f#^% is alice?
woman(janice).
man(peter).
Now if we would like to have all attending women we can simply query "woman(X)." to obtain this
---
# What do we use it for?
* Suited for situations in which rule-based logical queries are used, for example databases or voice processing systems.
* It originated from first-order logic and is, unlike most programming languages, declarative.
This means that the flow is expressed in terms of relation represented as fact and rules.
By running a query over these relations, a computation can be done.

---
* Is the language statically typed?
* Are effects such as assignments, controlled?
* Is the language call by value? Call by name? Call by need?
* Does prolog have reference semantics? Or value semantics?
* Does it have algebraic data types? Objects? Structs? Records?
* How does the Prolog engine find solutions?
* What is cut?
* When should Prolog be used?
---
# Titles

You can use the hash symbol \# to make the title of a slide.

## Subtitle

You can use more than one hash symbol \#\# to have subtitles on your
slide.

---

* Bullet lists
* are pretty easy
* too!

---

# Emphasis

You can include a word in asterisks to add *emphasis* or two asterisks
to make it **bold**.

That is:

```
*emphasis* vs **bold**
```

---

# Images

Please include any images in the `img` subdirectory.

You can refer to images using the usual markdown syntax:

![My caption](img/uueduc.jpg "Alt caption"){ width=30% }

![My caption](img/henk.pdf "Alt caption"){ width=100% }

---
# Staged builds

This is quite easy 1

. . .

Just insert `. . .` on a new line when you want the slide to appear
incrementally.

---

# Code

You can use backticks to include inline code such as `x` or `y`.

Use three backticks to introduce a code block:

```
main = print "Hello world!"
```

---

# Syntax highlighting

There are syntax highlighting options for the most widely used
languages.

```haskell
foo y = let x = 4 in x + z
  where
  z = 12
```

---

# Making slides

I've included a Makefile to build slides.

You will need to have the Haskell tool `pandoc` installed:

```
> cabal install pandoc
> make
```
---

# Working with markdown

You may want to install the markdown mode for emacs (or some other
editor of choice).

I've included some file local variables at the bottom of this file --
you may find them useful.

---

# Inline latex

You can always use \emph{inline} \LaTeX commands if you want.

But try to avoid this if you can.

Most Markdown commands should suffice.

\LaTeX is useful for formula's

\begin{equation}
\tau + x = \sigma
\end{equation}

Or inline formulas, enclosed in dollar symbols like so $\tau + x$.

---

# Questions

If you can't get things to work, don't hesitate to get in touch!


<!-- Local Variables:  -->
<!-- pandoc/write: beamer -->
<!-- pandoc/latex-engine: "xelatex" -->
<!-- pandoc/template: "beamer-template.tex" -->
<!-- End:  -->

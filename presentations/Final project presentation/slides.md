---
author: Winand, Roald, Sjoerd, Alexey and Anvar
title: Concepts of programming languages
subtitle: Embedding Prolog in C#
theme: uucs
mainfont: Ubuntu Light
sansfont: Ubuntu Light
---

# Contents

* Problem definition  1 min 
* Methodolology       1 min
* Implementation      6 min  4s
* Results             2 min  3s
* Reflection          2 min  


# Problem definition


# Methodolology

* load in terms and clauses
* implement unification
* implement backtracking, which will be a milestone
* implement mathematics 
* implement native Prolog functions such as findall
* implement negation and cut 

# Implementation
Say we want to have a database with guests of a party, we can denote in the database our guests and their resp. gender in the following way.
woman(jody).
woman(alice). # alice? who the f#^% is alice?
woman(janice).
man(peter).
Now if we would like to have all attending women we can simply query "woman(X)." to obtain this



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

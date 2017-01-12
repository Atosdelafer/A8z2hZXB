
default:slides.pdf

slides.pdf:slides.md
	pandoc -t beamer slides.md -o slides.pdf --latex-engine=xelatex --template beamer-template.tex

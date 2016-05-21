@echo off

for /l %%n in (1,0,2) do (
  echo pushing
  git push
)
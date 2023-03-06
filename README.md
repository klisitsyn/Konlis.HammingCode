# Konlis.HammingCode

Practice implementation of Hamming code calculator, based off Wiki page:
- https://en.wikipedia.org/wiki/Hamming_code

Works on arbitrarily large (within reason) input data size.
Given that algorithm complexity is O(N^2), it's impractical on large data blocks (greater than few dozen or hundred bits).

**WARNING**: overflow checks are not implemented, so it may break on very long input.
Though likely the computer will run out of memory first.
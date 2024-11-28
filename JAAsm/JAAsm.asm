; SHA256Asm.asm

global Sigma0Asm
global Sigma1Asm

section .text

; Sigma0 implementation
Sigma0Asm:
    ; Sigma0(x) = ROTR(x, 7) ^ ROTR(x, 18) ^ (x >> 3)
    ; Arguments: uint x (passed in rdi)
    mov eax, edi            ; Copy x to eax
    ror eax, 7              ; ROTR(x, 7)
    mov ebx, eax            ; Store result in ebx

    mov eax, edi            ; Copy x to eax again
    ror eax, 18             ; ROTR(x, 18)
    xor eax, ebx            ; XOR with previous result

    ; (x >> 3)
    mov ebx, edi
    shr ebx, 3
    xor eax, ebx            ; XOR with (x >> 3)

    ret

; Sigma1 implementation
Sigma1Asm:
    ; Sigma1(x) = ROTR(x, 17) ^ ROTR(x, 19) ^ (x >> 10)
    ; Arguments: uint x (passed in rdi)
    mov eax, edi            ; Copy x to eax
    ror eax, 17             ; ROTR(x, 17)
    mov ebx, eax            ; Store result in ebx

    mov eax, edi            ; Copy x to eax again
    ror eax, 19             ; ROTR(x, 19)
    xor eax, ebx            ; XOR with previous result

    ; (x >> 10)
    mov ebx, edi
    shr ebx, 10
    xor eax, ebx            ; XOR with (x >> 10)

    ret

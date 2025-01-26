; SHA256Asm.asm
.code
; Export functions
PUBLIC Sigma0Asm
PUBLIC Sigma1Asm
PUBLIC BigSigma0Asm
PUBLIC BigSigma1Asm
PUBLIC ChAsm
PUBLIC MajAsm
PUBLIC ROTRAsm

; uint ROTR(uint x, int n)
ROTRAsm PROC
    mov eax, ecx    ; first parameter (x)
    mov cl, dl      ; second parameter (n)
    ror eax, cl
    ret
ROTRAsm ENDP

; uint Sigma0Asm(uint x)
Sigma0Asm PROC
     ; Za³aduj dane do rejestru ymm0
    movdqu xmm0, xmmword ptr [rcx]    ; Za³aduj dane z pamiêci (x) do ymm0

    ; Przesuniêcia w prawo
    vpsrld xmm1, xmm0, 7            ; Przesuniêcie w prawo o 7 bitów
    vpsrld xmm2, xmm0, 18              ; Przesuniêcie w prawo o 18 bitów
    vpsrld xmm3, xmm0, 3               ; Przesuniêcie w prawo o 3 bity

    ; Operacje XOR
    vpxor xmm0, xmm1, xmm2            ; XOR miêdzy wynikami przesuniêæ
    vpxor xmm0, xmm0, xmm3            ; XOR z kolejnym wynikiem

    ; Zwróæ wynik (xored wyniki)
    ret
Sigma0Asm ENDP

; uint Sigma1Asm(uint x)
Sigma1Asm PROC
    vmovdqu ymm0, ymmword ptr [rcx]    ; Za³aduj dane do ymm0
    vpsrld ymm1, ymm0, 17              ; Przesuniêcie w prawo o 17 bitów
    vpsrld ymm2, ymm0, 19              ; Przesuniêcie w prawo o 19 bitów
    vpsrld ymm3, ymm0, 10              ; Przesuniêcie w prawo o 10 bitów

    vpxor ymm0, ymm1, ymm2            ; XOR miêdzy tymi danymi
    vpxor ymm0, ymm0, ymm3            ; XOR z kolejnym wynikiem

    ret
Sigma1Asm ENDP

; uint BigSigma0Asm(uint x)
BigSigma0Asm PROC
    mov eax, ecx    ; input parameter
    mov r10d, eax
    ror eax, 2
    ror r10d, 13
    xor eax, r10d
    ror r10d, 22
    xor eax, r10d
    ret
BigSigma0Asm ENDP

; uint BigSigma1Asm(uint x)
BigSigma1Asm PROC
    mov eax, ecx    ; input parameter
    mov r10d, eax
    ror eax, 6
    ror r10d, 11
    xor eax, r10d
    ror r10d, 25
    xor eax, r10d
    ret
BigSigma1Asm ENDP

; uint Ch(uint x, uint y, uint z)
ChAsm PROC 
    mov eax, ecx    ; x 
    mov r10d, edx   ; y 
    mov r11d, r8d   ; z 
    not eax         ; equivalent to ~x
    and eax, r11d   ; (~x & z)
    mov r9d, ecx    ; x
    and r9d, r10d   ; (x & y)
    xor eax, r9d    ; final XOR operation
    ret 
ChAsm ENDP

; uint Maj(uint x, uint y, uint z)
MajAsm PROC 
    mov eax, ecx    ; x 
    mov r10d, edx   ; y 
    mov r11d, r8d   ; z 
    mov r9d, eax    ; x
    and r9d, r10d   ; (x & y)
    mov r8d, eax    ; x
    and r8d, r11d   ; (x & z)
    or r9d, r8d     ; (x & y) | (x & z)
    mov r8d, r10d   ; y
    and r8d, r11d   ; (y & z)
    or eax, r9d     ; (x & y) | (x & z) | (y & z)
    ret 
MajAsm ENDP

end
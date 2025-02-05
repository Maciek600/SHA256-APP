.code
PUBLIC Sigma0Asm
PUBLIC Sigma1Asm
PUBLIC BigSigma0Asm
PUBLIC BigSigma1Asm
PUBLIC ChAsm
PUBLIC MajAsm
PUBLIC ROTRAsm

ROTRAsm PROC
    mov eax, ecx        ; first parameter (x)
    mov cl, dl          ; second parameter (n)
    ror eax, cl
    ret
ROTRAsm ENDP

; void Sigma0Asm(ref uint x)
Sigma0Asm PROC
    push rbx
    mov ebx, [rcx]     ; Load x value
    
    ; Calculate ROTR(x, 7)
    mov eax, ebx
    ror eax, 7
    
    ; Calculate ROTR(x, 18)
    mov r8d, ebx
    ror r8d, 18
    
    ; Calculate SHR(x, 3)
    mov r9d, ebx
    shr r9d, 3
    
    ; XOR all results
    xor eax, r8d
    xor eax, r9d
    
    ; Store result back to memory
    mov [rcx], eax
    
    pop rbx
    ret
Sigma0Asm ENDP

; void Sigma1Asm(ref uint x)
Sigma1Asm PROC
    push rbx
    mov ebx, [rcx]     ; Load x value
    
    ; Calculate ROTR(x, 17)
    mov eax, ebx
    ror eax, 17
    
    ; Calculate ROTR(x, 19)
    mov r8d, ebx
    ror r8d, 19
    
    ; Calculate SHR(x, 10)
    mov r9d, ebx
    shr r9d, 10
    
    ; XOR all results
    xor eax, r8d
    xor eax, r9d
    
    ; Store result back to memory
    mov [rcx], eax
    
    pop rbx
    ret
Sigma1Asm ENDP

; uint BigSigma0Asm(uint x)
BigSigma0Asm PROC
    mov eax, ecx        ; Load x
    
    ; Calculate ROTR(x, 2)
    mov r8d, eax
    ror r8d, 2
    
    ; Calculate ROTR(x, 13)
    mov r9d, eax
    ror r9d, 13
    
    ; Calculate ROTR(x, 22)
    mov r10d, eax
    ror r10d, 22
    
    ; XOR all results
    mov eax, r8d
    xor eax, r9d
    xor eax, r10d
    
    ret
BigSigma0Asm ENDP

; uint BigSigma1Asm(uint x)
BigSigma1Asm PROC
    mov eax, ecx        ; Load x
    
    ; Calculate ROTR(x, 6)
    mov r8d, eax
    ror r8d, 6
    
    ; Calculate ROTR(x, 11)
    mov r9d, eax
    ror r9d, 11
    
    ; Calculate ROTR(x, 25)
    mov r10d, eax
    ror r10d, 25
    
    ; XOR all results
    mov eax, r8d
    xor eax, r9d
    xor eax, r10d
    
    ret
BigSigma1Asm ENDP

ChAsm PROC
    push rbx
    
    ; Za³aduj wartoœci z pamiêci (przez referencjê)
    mov ebx, [rcx]     ; x
    mov eax, [rdx]     ; y
    mov r8d, [r8]      ; z
    
    ; Umieœæ wartoœci w xmm0
    vmovd xmm0, ebx
    vpinsrd xmm0, xmm0, eax, 1
    vpinsrd xmm0, xmm0, r8d, 2

    ; ~x 
    vpcmpeqd xmm1, xmm1, xmm1   ; Ustaw wszystkie bity na 1
    vpxor xmm1, xmm1, xmm0      ; ~x w xmm1

    ; Oblicz (x & y)
    vpsrldq xmm2, xmm0, 4       ; y na miejsce x
    vpand xmm2, xmm2, xmm0      ; xmm2 = x & y

    ; Oblicz (~x & z)
    vpsrldq xmm3, xmm0, 8       ; z na miejsce x
    vpand xmm1, xmm1, xmm3      ; xmm1 = ~x & z

    ; Po³¹cz wyniki w xmm0
    vpxor xmm0, xmm1, xmm2

    
    vmovd eax, xmm0
    mov [rcx], eax

    pop rbx
    ret
ChAsm ENDP


MajAsm PROC
    ; Za³aduj wartoœci do rejestru XMM0
    vmovd xmm0, ecx
    vpinsrd xmm0, xmm0, edx, 1
    vpinsrd xmm0, xmm0, r8d, 2
    

    ; Utwórz kopie wartoœci y i z
    vpsrldq xmm1, xmm0, 4    ; y
    vpsrldq xmm2, xmm0, 8    ; z

    ; Oblicz x & y
    vpand xmm3, xmm0, xmm1   ; xmm3 = x & y

    ; Oblicz x & z
    vpand xmm4, xmm0, xmm2   ; xmm4 = x & z

    ; Oblicz y & z
    vpand xmm5, xmm1, xmm2   ; xmm5 = y & z

    ; Po³¹cz wyniki: (x & y) ^ (x & z) ^ (y & z)
    vpxor xmm3, xmm3, xmm4   ; xmm3 = (x & y) ^ (x & z)
    vpxor xmm0, xmm3, xmm5   ; xmm0 = (x & y) ^ (x & z) ^ (y & z)

    ; Zwróæ wynik w eax
    vmovd eax, xmm0
    ret
MajAsm ENDP



end
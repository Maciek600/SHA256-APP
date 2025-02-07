;----------------------------
;   JA PROJEKT
;   SHA-256 ASM
;   autor: Maciej Fajlhauer
;   INF-KTW 1/5
;   ROK AKADEMICKI 2024-25
;----------------------------

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
    push rbx                        ; Zachowanie rejestru rbx na stosie
    
    ; £adowanie argumentów z pamiêci
    mov ebx, [rcx]                  ; Pierwszy argument (x) do ebx
    mov eax, [rdx]                  ; Drugi argument (y) do eax
    mov r8d, [r8]                   ; Trzeci argument (z) do r8d
    
    ; Przygotowanie wartoœci w rejestrze XMM0
    vmovd xmm0, ebx                 ; Przeniesienie x do xmm0
    vpinsrd xmm0, xmm0, eax, 1      ; Dodanie y do xmm0[1]
    vpinsrd xmm0, xmm0, r8d, 2      ; Dodanie z do xmm0[2]
    
    ; Obliczenie negacji x (~x)
    vpcmpeqd xmm1, xmm1, xmm1       ; Wszystkie bity na 1 (for NOT operation)
    vpxor xmm1, xmm1, xmm0          ; Negacja x w xmm1
    
    ; Obliczenie czêœci (x & y)
    vpsrldq xmm2, xmm0, 4           ; Przesuniêcie y na pozycjê x
    vpand xmm2, xmm2, xmm0          ; Wykonanie x & y
    
    ; Obliczenie czêœci (~x & z)
    vpsrldq xmm3, xmm0, 8           ; Przesuniêcie z na pozycjê x
    vpand xmm1, xmm1, xmm3          ; Wykonanie ~x & z
    
    ; Koñcowe obliczenie Ch(x,y,z) = (x & y) ^ (~x & z)
    vpxor xmm0, xmm1, xmm2          ; Po³¹czenie wyników XORem
    
    ; Przygotowanie wyniku
    vmovd eax, xmm0                 ; Przeniesienie wyniku do eax
    mov [rcx], eax                  ; Zapisanie wyniku pod adresem pierwszego argumentu
    pop rbx                         ; Przywrócenie rbx ze stosu
    ret                            ; Powrót z procedury
ChAsm ENDP

MajAsm PROC
    ; Przygotowanie argumentów w XMM0
    vmovd xmm0, ecx                 ; Przeniesienie x do xmm0
    vpinsrd xmm0, xmm0, edx, 1      ; Dodanie y do xmm0[1]
    vpinsrd xmm0, xmm0, r8d, 2      ; Dodanie z do xmm0[2]
    
    ; Utworzenie kopii argumentów
    vpsrldq xmm1, xmm0, 4           ; Kopia y
    vpsrldq xmm2, xmm0, 8           ; Kopia z
    
    ; Obliczenie wszystkich kombinacji AND
    vpand xmm3, xmm0, xmm1          ; x & y w xmm3
    vpand xmm4, xmm0, xmm2          ; x & z w xmm4
    vpand xmm5, xmm1, xmm2          ; y & z w xmm5
    
    ; Koñcowe obliczenie Maj(x,y,z) = (x & y) ^ (x & z) ^ (y & z)
    vpxor xmm3, xmm3, xmm4          ; (x & y) ^ (x & z)
    vpxor xmm0, xmm3, xmm5          ; Dodanie (y & z)
    
    ; Zwrócenie wyniku
    vmovd eax, xmm0                 ; Przeniesienie wyniku do eax
    ret                            ; Powrót z procedury
MajAsm ENDP




end
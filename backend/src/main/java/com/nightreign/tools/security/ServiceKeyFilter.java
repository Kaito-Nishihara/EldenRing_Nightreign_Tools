package com.nightreign.tools.security;

import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.annotation.Order;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.time.LocalDateTime;

/**
 * サービスキー認証フィルター
 * Gateway(.NET)からのリクエストに含まれる X-Service-Key ヘッダーを検証する。
 * ヘッダーが存在しない、または値が一致しない場合は 401 Unauthorized を返す。
 * Swagger UI と OpenAPI ドキュメントへのアクセスは認証不要とする。
 */
@Component
@Order(1)
public class ServiceKeyFilter extends OncePerRequestFilter {

    private static final String SERVICE_KEY_HEADER = "X-Service-Key";

    @Value("${app.service-key}")
    private String serviceKey;

    @Override
    protected void doFilterInternal(HttpServletRequest request,
                                    HttpServletResponse response,
                                    FilterChain filterChain)
            throws ServletException, IOException {

        // Swagger UI・OpenAPIドキュメントは認証をスキップ
        String path = request.getRequestURI();
        if (isSwaggerPath(path)) {
            filterChain.doFilter(request, response);
            return;
        }

        String headerValue = request.getHeader(SERVICE_KEY_HEADER);

        if (headerValue == null || !isEqual(headerValue, serviceKey)) {
            response.setStatus(HttpStatus.UNAUTHORIZED.value());
            response.setContentType(MediaType.APPLICATION_JSON_VALUE);
            response.setCharacterEncoding("UTF-8");
            response.getWriter().write(
                    String.format(
                            "{\"status\":401,\"error\":\"Unauthorized\",\"message\":\"サービスキーが無効です\",\"timestamp\":\"%s\"}",
                            LocalDateTime.now()
                    )
            );
            return;
        }

        filterChain.doFilter(request, response);
    }

    /**
     * タイミング攻撃を防ぐための定数時間比較
     */
    private boolean isEqual(String a, String b) {
        return MessageDigest.isEqual(
                a.getBytes(StandardCharsets.UTF_8),
                b.getBytes(StandardCharsets.UTF_8)
        );
    }

    /**
     * Swagger UI関連パスかどうかを判定する
     */
    private boolean isSwaggerPath(String path) {
        return path.startsWith("/swagger-ui")
                || path.startsWith("/v3/api-docs")
                || path.equals("/swagger-ui.html");
    }
}

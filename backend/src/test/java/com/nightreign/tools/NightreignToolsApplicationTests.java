package com.nightreign.tools;

import org.junit.jupiter.api.Test;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.ActiveProfiles;

/**
 * アプリケーション起動テスト
 */
@SpringBootTest
@ActiveProfiles("test")
class NightreignToolsApplicationTests {

    @Test
    void contextLoads() {
        // アプリケーションコンテキストが正常にロードされることを検証
    }
}

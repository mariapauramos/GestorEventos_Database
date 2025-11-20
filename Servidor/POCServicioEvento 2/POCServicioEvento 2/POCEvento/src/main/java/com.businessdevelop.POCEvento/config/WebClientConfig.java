package com.businessdevelop.POCEvento.config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.web.client.RestTemplateBuilder;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.client.RestTemplate;
import org.springframework.web.reactive.function.client.ExchangeStrategies;
import org.springframework.web.reactive.function.client.WebClient;

@Configuration
public class WebClientConfig {

    @Bean
    public WebClient webClient(
            @Value("${sede.service.url}") String baseurl,
            @Value("${sede.service.username}") String username,
            @Value("${sede.service.password}") String password) {

        return WebClient.builder()
                .baseUrl(baseurl)
                .defaultHeaders(headers -> headers.setBasicAuth(username, password))
                .exchangeStrategies(
                        ExchangeStrategies.builder()
                                .codecs(config -> config
                                        .defaultCodecs()
                                        .maxInMemorySize(4 * 1024 * 1024)) // 4MB
                                .build())
                .build();
    }
}

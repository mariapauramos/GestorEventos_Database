package com.businessdevelop.POCEvento.config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.web.client.RestTemplateBuilder;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.client.RestTemplate;

@Configuration
public class RestTemplateConfig {

    @Bean
    public RestTemplate restTemplate(RestTemplateBuilder builder,
                                     @Value("${sede.service.username}") String username,
                                     @Value("${sede.service.password}") String password) {
        return builder
                .basicAuthentication(username, password)
                .build();
    }
}

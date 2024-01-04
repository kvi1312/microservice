- lệnh chạy docker : docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
- lệnh drop docker : docker-compose down
- account portainer http://local:9000 : admin - Nguyenkhai2611!
- account kibana http://localhost:5601 : elastic - admin
- rabbitMq http://localhost:15672 : guest - guest

# Package

- seriLogger : dùng để ghi log tập trung, có thể dùng cho nhiều app reference tới nó để sử dụng chung

- Elastic Search là một cơ sở dữ liệu tối ưu cho việc tìm kiếm. Nó cung cấp một bộ máy tìm kiếm dạng phân tán, có đầy đủ công cụ với một giao diện web HTTP có hỗ trợ dữ liệu JSON. Elastic Search đi kèm với với Kibana, Logstash tạo thành ELK Stack  
   =>>>>>>>>> https://200lab.io/blog/elastic-search-la-gi/

- ELK Stack (elastic search + logstash + kibana) : hệ sinh thái để ghi log về lỗi, hiệu xuất, và giúp giám sát cơ sở hạ tầng, hỗ trợ search engine

# TẠO INFRASTRACTURE CHO PROJECT

- abstract class : không cho phép tạo new instance
- DateTimeOffset tốt hơn DateTime trong việc convert từ utc sang local time zone
- đổi port product api aplication sang 5002
- tạo thư

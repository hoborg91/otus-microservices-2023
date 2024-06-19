{{- define "db-service-name" -}}
{{- .Release.Name }}-db-service
{{- end }}

{{- define "db-service-node-port" -}}
{{- .Values.dbService.nodePort }}
{{- end }}

{{- define "db-migration-0-name" -}}
{{- .Release.Name }}-db-migration-0
{{- end }}

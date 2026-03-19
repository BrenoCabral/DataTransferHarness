# Medplum Bot Basics

## Overview
Medplum bots are serverless TypeScript functions that execute in response to events or HTTP requests. They have access to the Medplum API and can interact with FHIR resources.

## Bot Handler Signature
```typescript
import { BotEvent, MedplumClient } from '@medplum/core';

export default async function handler(
  medplum: MedplumClient,
  event: BotEvent
): Promise<any> {
  // Bot logic here
}
```

## Key Concepts

### MedplumClient
The `medplum` parameter provides access to the Medplum API:
- `medplum.createResource()` - Create FHIR resources
- `medplum.updateResource()` - Update existing resources
- `medplum.searchResources()` - Search for resources
- `medplum.readResource()` - Read a specific resource

### BotEvent
The `event` parameter contains:
- `event.input` - The input data (often a FHIR resource or Bundle)
- `event.contentType` - The content type of the input
- `event.secrets` - Secure configuration values

## Best Practices
1. Always validate input data
2. Use try-catch for error handling
3. Log important operations
4. Return meaningful results
5. Keep bots focused on a single responsibility
